using EasyAnalysis.Models;
using EasyAnalysis.Repository;
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
        public IDropDownFieldRepository _dropDownFieldRepository;

        public RepositoryController()
        {
            _dropDownFieldRepository = new DropDownFieldRepository();
        }

        /// <summary>
        /// api/repository
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RepositoryModel> Get()
        {
            var repositories = new List<RepositoryModel>
            {
                new RepositoryModel { Code = "UWP", Text = "Universal Windows Platform"},
                new RepositoryModel { Code = "SOUWP", Text = "UWP@Stackoverflow"},
                new RepositoryModel { Code = "SOTFS", Text = "Team Foundation Service"},
                new RepositoryModel { Code = "SOVSO", Text = "Visual Studio Online"},
                new RepositoryModel { Code = "OFFICE", Text = "Microsoft Office"},
                new RepositoryModel { Code = "OFFICEDEV", Text = "Office for Developers"},
                new RepositoryModel { Code = "SQL", Text = "Microsoft SQL Server"},
                new RepositoryModel { Code = "SOO365", Text = "Office 365 for Developers"},
            };

            return repositories;
        }

        [Route("api/repository/fields")]
        public HttpResponseMessage GetRepositoryFields([FromUri] string name)
        {
            name = name.ToLower();

            if (name.Equals("souwp"))
            {
                name = "uwp";
            }

            var groups = _dropDownFieldRepository.ListByRepository(name);

            return Request.CreateResponse(HttpStatusCode.OK, groups, new JsonMediaTypeFormatter());
        }
    }
}
