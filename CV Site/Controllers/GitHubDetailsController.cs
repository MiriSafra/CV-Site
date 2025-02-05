using Microsoft.AspNetCore.Mvc;
using Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CV_Site.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubDetailsController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public GitHubDetailsController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        // GET: api/<GitHubDetailsController>/portfolio
        [HttpGet("portfolio")]
        public async Task<IActionResult> GetPortfolio()
        {
            var portfolio = await _gitHubService.GetPortfolio();
            if (portfolio == null || portfolio.Count == 0)
            {
                return NotFound("No repositories found.");
            }
            return Ok(portfolio);
        }

        // GET: api/<GitHubDetailsController>/followers/{userName}
        [HttpGet("followers/{userName}")]
        public async Task<IActionResult> GetUserFollowersAsync(string userName)
        {
            var followersCount = await _gitHubService.GetUserFollowersAsync(userName);
            return Ok(new { UserName = userName, FollowersCount = followersCount });
        }

        // GET: api/<GitHubDetailsController>/searchRepositories
        [HttpGet("searchRepositories")]
        public async Task<IActionResult> SearchRepositories([FromQuery] string repoName = "", [FromQuery] string language = "", [FromQuery] string user = "")
        {
            var repositories = await _gitHubService.SearchRepositoriesAsync(repoName, language, user);
            if (repositories == null || repositories.Count() == 0)
            {
                return NotFound("No repositories found matching the search criteria.");
            }
            return Ok(repositories);
        }
    }
}
