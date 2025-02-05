using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RepositoryDetails
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stars { get; set; }
        public DateTime LastCommit { get; set; }
        public List<string> Languages { get; set; }
        public int PullRequests { get; set; }
        public string HtmlUrl { get; set; }
        public string Owner { get; set; }
        public string Language { get; set; }
    }

}
