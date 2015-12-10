using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace EasyAnalysis.Controllers
{
    public class RepositoryController : ApiController
    {
        [Route("api/repository/fields")]
        public HttpResponseMessage GetRepositoryFields([FromUri] string name)
        {
            var groups = new List<DropDownField>();

            name = name.ToLower();

            if (name.Equals("uwp") || name.Equals("souwp"))
            {
                groups = new List<DropDownField> {
                new DropDownField
                {
                    Name = "platform",
                    DisplayName = "Platform",
                    Options = new List<Option>
                    {
                        new Option { Value = "", Name = "All" },
                        new Option { Value = "uwp", Name = "Universal Windows Platform" },
                        new Option { Value = "wp8.1", Name = "Windows Phone 8.1" },
                        new Option { Value = "w8.1", Name = "Windows 8.1" },
                        new Option { Value = "wpsl", Name = "Windows Phone Silverlight" },
                    }
                },
                new DropDownField
                {
                    Name = "language",
                    DisplayName = "Language",
                    Options = new List<Option>
                    {
                        new Option { Value = "", Name = "All" },
                        new Option { Value = "c#", Name = "C#" },
                        new Option { Value = "c++", Name = "C++" },
                        new Option { Value = "vb", Name = "VB.NET" },
                        new Option { Value = "javascript", Name = "Javascript" },
                    }
                } };
            }
            else if (name.Equals("sotfs"))
            {
                groups = new List<DropDownField> {
                new DropDownField
                {
                    Name = "tfs_version",
                    DisplayName = "TFS Version",
                    Options = new List<Option>
                    {
                        new Option { Value = "", Name = "Unknown" },
                        new Option { Value = "tfs2008", Name = "TFS 2008" },
                        new Option { Value = "tfs2010", Name = "TFS 2010" },
                        new Option { Value = "tfs2012", Name = "TFS 2012" },
                        new Option { Value = "tfs2013", Name = "TFS 2013" },
                        new Option { Value = "tfs2015", Name = "TFS 2015" },
                        new Option { Value = "tfs2015sp1", Name = "TFS 2015 SP1" }
                    }
                },
                new DropDownField
                {
                    Name = "issue_type",
                    DisplayName = "Issue Type",
                    Options = new List<Option>
                    {
                        new Option { Value = "", Name = "Unknown" },
                        new Option { Value = "how-to", Name = "How To" },
                        new Option { Value = "troubleshooting", Name = "Troubleshooting" },
                        new Option { Value = "general-discussion", Name = "General Discussion" },
                        new Option { Value = "administrator-related", Name = "Administrator Related" },
                    }
                },
                new DropDownField
                {
                    Name = "voc",
                    DisplayName = "Voice of Customer",
                    Options = new List<Option>
                    {
                        new Option { Value = "", Name = "Unknown" },
                        new Option { Value = "content-request", Name = "Content Request" },
                        new Option { Value = "sample-request", Name = "Sample Request" },
                        new Option { Value = "feature-request", Name = "Feature Request" },
                        new Option { Value = "potential-bug", Name = "Potential Bug" },
                    }
                }
                };
            } else if (name.Equals("sovso"))
            {
                groups = new List<DropDownField> {
                new DropDownField
                {
                    Name = "issue_type",
                    DisplayName = "Issue Type",
                    Options = new List<Option>
                    {
                        new Option { Value = "", Name = "Unknown" },
                        new Option { Value = "how-to", Name = "How To" },
                        new Option { Value = "troubleshooting", Name = "Troubleshooting" },
                        new Option { Value = "general-discussion", Name = "General Discussion" },
                        new Option { Value = "administrator-related", Name = "Administrator Related" },
                    }
                },
                new DropDownField
                {
                    Name = "voc",
                    DisplayName = "Voice of Customer",
                    Options = new List<Option>
                    {
                        new Option { Value = "", Name = "Unknown" },
                        new Option { Value = "content-request", Name = "Content Request" },
                        new Option { Value = "sample-request", Name = "Sample Request" },
                        new Option { Value = "feature-request", Name = "Feature Request" },
                        new Option { Value = "potential-bug", Name = "Potential Bug" },
                    }
                }
                };
            }

            return Request.CreateResponse(HttpStatusCode.OK, groups, new JsonMediaTypeFormatter());
        }
    }
}
