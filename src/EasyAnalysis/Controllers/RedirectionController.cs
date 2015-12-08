using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;

namespace EasyAnalysis.Controllers
{
    public class RedirectionController : Controller
    {
        public ActionResult Index()
        {
            throw new NotImplementedException();
        }


        public ActionResult Goto(string id)
        {
            id = id.ToLower();

            var url = id.StartsWith("so_")
                ? string.Format("http://stackoverflow.com/questions/{0}", id.Replace("so_", "").Trim())
                : string.Format("https://social.msdn.microsoft.com/Forums/windowsapps/en-US/{0}", id.Trim());

            return Redirect(url);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">guid | number</param>
        /// <param name="exteranl">tool name</param>
        /// <returns></returns>
        public ActionResult Navigate(string id, string external)
        {
            var identifier = string.Empty;

            if (external == "sotool")
            {
                identifier = string.Format("SO_{0}", id);

            }
            else if (external == "mt")
            {
                identifier = id;
            }
            else {
                return Redirect("/#/");
            }

            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var connection = new SqlConnection(cs))
            {
                var repository = connection.Query("SELECT [Repository] As [Name] FROM [dbo].[VwThreads] WHERE[Id] = @Id", new { Id = identifier })
                                           .FirstOrDefault();

                // If the instance is not found in the system, return to home page
                if (repository == null)
                {
                    return Redirect("/#/");
                }

                var url = UrlHelper.GenerateContentUrl(string.Format("~/#/detail/{0}/{1}", repository.Name, identifier), HttpContext);

                return Redirect(url);
            }
        }

    }
}