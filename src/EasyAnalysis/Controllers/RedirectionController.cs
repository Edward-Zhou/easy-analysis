using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using EasyAnalysis.Repository;
using System.Threading.Tasks;
using Utility.MSDN;
using EasyAnalysis.Models;

namespace EasyAnalysis.Controllers
{
    public class RedirectionController : Controller
    {
        private readonly IThreadRepository _threadRepository;

        public RedirectionController()
        {
            var context = new DefaultDbConext();
            _threadRepository = new ThreadRepository(context);
        }

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
        public async Task<ActionResult> Navigate(string id, string external, string type)
        {
            var identifier = string.Empty;

            if (external == "sotool")
            {
                identifier = string.Format("SO_{0}", id);

            }
            else if (external == "mt")
            {
                identifier = id;

                // create new If not exists
                if (!_threadRepository.Exists(identifier))
                {
                    await RegisterNewThreadAsync(identifier);
                }
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

                if (type == "iframe")
                {
                    url = UrlHelper.GenerateContentUrl(string.Format("~/frame.html#/embed/{0}/{1}", repository.Name, identifier), HttpContext);
                }

                return Redirect(url);
            }
        }


        private async Task<bool> RegisterNewThreadAsync(string identifier)
        {
            // register a new thread item
            try
            {
                var parser = new ThreadParser(Guid.Parse(identifier));

                var info = await parser.ReadThreadInfoAsync();

                if (info == null)
                {
                    return false;
                }

                // query the database by the identifer / create a new item if not exist
                var model = new ThreadModel
                {
                    Id = info.Id,
                    Title = info.Title,
                    AuthorId = info.AuthorId,
                    CreateOn = info.CreateOn,
                    ForumId = info.ForumId
                };

                _threadRepository.Create(model);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}