using Microsoft.Extensions.Caching.Memory;
using Octokit;
using Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CV_Site.CachedServices
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private const string UserPortfolio = "UserPortfolioKey";

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }

        public async Task<List<RepositoryDetails>> GetPortfolio()
        {
            if (_memoryCache.TryGetValue(UserPortfolio, out List<RepositoryDetails> portfolio))
            {
                return portfolio;
            }

            var cacheOptions = new MemoryCacheEntryOptions()
                               .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                               .SetSlidingExpiration(TimeSpan.FromSeconds(10));

            portfolio = await _gitHubService.GetPortfolio();

            _memoryCache.Set(UserPortfolio, portfolio, cacheOptions);

            return portfolio ?? new List<RepositoryDetails>();
        }

        public Task<int> GetUserFollowersAsync(string userName)
        {
            return _gitHubService.GetUserFollowersAsync(userName);
        }

        public Task<List<RepositoryDetails>> SearchRepositoriesAsync(string repoName = "", string language = "", string user = "")
        {
            return _gitHubService.SearchRepositoriesAsync(repoName, language, user);
        }
    }
}