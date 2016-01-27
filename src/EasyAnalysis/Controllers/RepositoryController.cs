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

        public IScopeRepository _scopeRepository;

        public RepositoryController()
        {
            _dropDownFieldRepository = new DropDownFieldRepository();

            _scopeRepository = new ScopeRepository();
        }

        /// <summary>
        /// api/repository
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ScopeModel> Get()
        {
            var scopes = _scopeRepository.List();

            return scopes;
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
