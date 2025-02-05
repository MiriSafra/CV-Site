using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface IGitHubService
    {
        Task<int> GetUserFollowersAsync(string userName);

        Task<List<RepositoryDetails>> GetPortfolio();

        Task<List<RepositoryDetails>> SearchRepositoriesAsync(string? repoName = null, string? language = null, string? user = null);
    }
}
