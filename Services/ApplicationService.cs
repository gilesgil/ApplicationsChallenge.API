using ApplicationsChallenge.API.Models;
using ApplicationsChallenge.API.Repositories;

namespace ApplicationsChallenge.API.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ApplicationStatusBackgroundService _backgroundService;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            ApplicationStatusBackgroundService backgroundService,
            ILogger<ApplicationService> logger)
        {
            _applicationRepository = applicationRepository;
            _backgroundService = backgroundService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all applications.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
        {
            return await _applicationRepository.GetAllAsync();
        }

        public async Task<Application?> GetApplicationByIdAsync(int id)
        {
            return await _applicationRepository.GetByIdAsync(id);
        }        public async Task<Application> CreateApplicationAsync(Application application, int userId)
        {
            application.UserId = userId;
            application.Date = DateTime.UtcNow;
            application.Status = "submitted";
            
            var createdApplication = await _applicationRepository.CreateAsync(application);
            
            // Register the application with the background service for automatic status update
            _backgroundService.RegisterApplication(createdApplication);
            _logger.LogInformation($"Created application {createdApplication.Id} and registered for automatic status update");
            
            return createdApplication;
        }
        
        public async Task<bool> UpdateApplicationStatusAsync(int id, string status)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
            {
                return false;
            }

            // If the application is being updated from "submitted" to another status,
            // remove it from the background service queue
            if (application.Status == "submitted" && status != "submitted")
            {
                _backgroundService.UnregisterApplication(id);
                _logger.LogInformation($"Removed application {id} from background processing due to manual status change");
            }

            application.Status = status;
            return await _applicationRepository.UpdateAsync(application);
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            // Remove from background service queue if it exists
            _backgroundService.UnregisterApplication(id);
            _logger.LogInformation($"Removed application {id} from background processing due to deletion");
            
            return await _applicationRepository.DeleteAsync(id);
        }
    }
}
