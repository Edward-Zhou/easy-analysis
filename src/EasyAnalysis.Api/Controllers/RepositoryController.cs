using EasyAnalysis.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyAnalysis.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class RepositoryController : ApiController
    {
        [Route("api/repository/fields")]
        public HttpResponseMessage GetRepositoryFields([FromUri] string name)
        {
            var groups = new List<DropDownField>
            {
                new DropDownField
                {
                    Name = "Platform",
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
                    Name = "Language",
                    DisplayName = "Language",
                    Options = new List<Option>
                    {
                        new Option { Value = "", Name = "All" },
                        new Option { Value = "c#", Name = "C#" },
                        new Option { Value = "c++", Name = "C++" },
                        new Option { Value = "vb", Name = "VB.NET" },
                        new Option { Value = "javascript", Name = "Javascript" },
                    }
                }
            };

            return Request.CreateResponse(HttpStatusCode.OK, groups, new JsonMediaTypeFormatter());
        }
    }
}
