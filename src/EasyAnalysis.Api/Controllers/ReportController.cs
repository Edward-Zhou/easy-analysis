using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyAnalysis.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ReportController : ApiController
    {
        [Route("api/report/{id}/run"), HttpGet]
        public HttpResponseMessage Run(string id)
        {
            var reportObject = new Reporting.CategoryReportObject();

            var nameValueParis = Request.GetQueryNameValuePairs();

            var nameValueDict = nameValueParis.ToDictionary(m => m.Key, m => m.Value);

            var result = reportObject.Run(nameValueDict);

            var jsonText = JsonConvert.SerializeObject(result,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            response.Content = new StringContent(jsonText, Encoding.UTF8, "application/json");

            return response;
        }
    }
}
