using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace Service
{
    public static class Extenstions
    {
        public static void AddGitHubIntegretions(this IServiceCollection services, Action<GitHubIntegrationOptions> configur)
        {
            services.Configure(configur);
            services.AddScoped<IGitHubService, GitHubService>();

        }
    }
}
