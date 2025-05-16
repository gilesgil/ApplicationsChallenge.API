using ApplicationsChallenge.API.Models;

namespace ApplicationsChallenge.API.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<Application>> GetAllApplicationsAsync();
        Task<Application?> GetApplicationByIdAsync(int id);
        Task<Application> CreateApplicationAsync(Application application, int userId);
        Task<bool> UpdateApplicationStatusAsync(int id, string status);
        Task<bool> DeleteApplicationAsync(int id);
    }
}
