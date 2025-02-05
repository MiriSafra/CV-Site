using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _options;

        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(_options.Token))
            {
                throw new ArgumentNullException(nameof(_options.Token), "GitHub Personal Access Token is not set.");
            }

            _client = new GitHubClient(new ProductHeaderValue("GitHubPortfolio"))
            {
                Credentials = new Credentials(_options.Token)
            };
        }

        public async Task<List<RepositoryDetails>> GetPortfolio()
        {
            if (string.IsNullOrEmpty(_options.Name))
            {
                throw new ArgumentException("GitHub username is not provided.");
            }

            var repositories = await _client.Repository.GetAllForUser(_options.Name);
            return await ConvertToRepositoryDetails(repositories);
        }

        public async Task<int> GetUserFollowersAsync(string userName)
        {
            var followers = await _client.User.Followers.GetAll(userName);
            return followers.Count;
        }

        public async Task<List<RepositoryDetails>> SearchRepositoriesAsync(string? query = null, string? language = null, string? user = null)
        {
            var searchTerms = new List<string>();

            if (!string.IsNullOrEmpty(query))
                searchTerms.Add(query);

            if (!string.IsNullOrEmpty(user))
                searchTerms.Add($"user:{user}");

            if (!searchTerms.Any())
                searchTerms.Add("stars:>=0");

            var searchQuery = string.Join(" ", searchTerms);
            Console.WriteLine($"Search query: {searchQuery}");

            var request = new SearchRepositoriesRequest(searchQuery);
            var result = await _client.Search.SearchRepo(request);
            var repositories = result.Items ?? new List<Repository>();

            if (!string.IsNullOrEmpty(language))
            {
                var languagesToSearch = language.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var filteredRepositories = await Task.WhenAll(repositories.Select(async repo =>
                {
                    var repoLanguages = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                    return languagesToSearch.All(lang => repoLanguages.Any(rl =>
                        string.Equals(rl.Name, lang, StringComparison.OrdinalIgnoreCase))) ? repo : null;
                }));

                repositories = filteredRepositories.Where(repo => repo != null).ToList();
            }

            return await ConvertToRepositoryDetails(repositories);
        }

        private async Task<List<RepositoryDetails>> ConvertToRepositoryDetails(IEnumerable<Repository> repositories)
        {
            var repoDetails = new List<RepositoryDetails>();

            foreach (var repo in repositories)
            {
                var languages = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                var pulls = await _client.Repository.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                repoDetails.Add(new RepositoryDetails
                {
                    Name = repo.Name,
                    Description = repo.Description,
                    Stars = repo.StargazersCount,
                    LastCommit = repo.PushedAt?.DateTime ?? repo.UpdatedAt.DateTime,
                    Languages = languages.Select(lang => lang.Name).ToList(),
                    PullRequests = pulls.Count,
                    HtmlUrl = repo.HtmlUrl,
                    Owner = repo.Owner.Login,
                    Language = repo.Language
                });
            }

            return repoDetails;
        }
    }
}
