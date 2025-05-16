using ApplicationsChallenge.API.Hubs;
using ApplicationsChallenge.API.Models;
using ApplicationsChallenge.API.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace ApplicationsChallenge.API.Services
{
    /// <summary>
    /// Background service to automatically update application statuses after a period of time
    /// </summary>
    public class ApplicationStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ApplicationStatusBackgroundService> _logger;
        private readonly ConcurrentDictionary<int, DateTime> _submittedApplications = new();
        private readonly TimeSpan _statusUpdateDelay = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);

        public ApplicationStatusBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<ApplicationStatusBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Register an application to be processed by the background service
        /// </summary>
        public void RegisterApplication(Application application)
        {
            if (application.Status == "submitted")
            {
                var completionTime = DateTime.UtcNow.Add(_statusUpdateDelay);
                _submittedApplications[application.Id] = completionTime;
                _logger.LogInformation($"Registered application {application.Id} for status update at {completionTime}");
            }
        }

        /// <summary>
        /// Remove an application from the background service processing queue
        /// </summary>
        public void UnregisterApplication(int applicationId)
        {
            if (_submittedApplications.TryRemove(applicationId, out _))
            {
                _logger.LogInformation($"Unregistered application {applicationId} from status update queue");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Application Status Background Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessApplicationsAsync();
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Application Status Background Service is stopping");
        }

        private async Task ProcessApplicationsAsync()
        {
            var now = DateTime.UtcNow;
            var readyApplications = _submittedApplications
                .Where(kvp => kvp.Value <= now)
                .Select(kvp => kvp.Key)
                .ToList();

            if (readyApplications.Count == 0)
            {
                return;
            }

            _logger.LogInformation($"Processing {readyApplications.Count} applications ready for status update");

            using var scope = _scopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ApplicationHub>>();

            foreach (var applicationId in readyApplications)
            {
                try
                {
                    // Remove from tracking dictionary
                    _submittedApplications.TryRemove(applicationId, out _);

                    // Get the application and check if it still exists and is in "submitted" status
                    var application = await applicationRepository.GetByIdAsync(applicationId);
                    if (application == null || application.Status != "submitted")
                    {
                        continue;
                    }

                    // Update status to completed
                    application.Status = "completed";
                    var success = await applicationRepository.UpdateAsync(application);

                    if (success)
                    {
                        _logger.LogInformation($"Updated application {applicationId} status to 'completed'");
                        
                        // Notify clients via SignalR
                        await hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", application);
                        
                        _logger.LogInformation($"Notified clients about application {applicationId} status update");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating status for application {applicationId}");
                    
                    // If there was an error, we'll try again next time
                    if (!_submittedApplications.ContainsKey(applicationId))
                    {
                        _submittedApplications[applicationId] = DateTime.UtcNow.AddSeconds(30);
                    }
                }
            }
        }

        /// <summary>
        /// Load all submitted applications from the database on service start
        /// </summary>
        public async Task InitializeFromDatabaseAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                
                var allApplications = await applicationRepository.GetAllAsync();
                var submittedApplications = allApplications.Where(a => a.Status == "submitted").ToList();
                
                if (submittedApplications.Any())
                {
                    _logger.LogInformation($"Loading {submittedApplications.Count} existing submitted applications");
                    
                    foreach (var application in submittedApplications)
                    {
                        // For existing applications, we'll schedule them to be processed soon (within 10 seconds)
                        // This ensures they don't all get processed at the same time and overload the system
                        var processingTime = DateTime.UtcNow.AddSeconds(10);
                        _submittedApplications[application.Id] = processingTime;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing applications from database");
            }
        }
    }
}
