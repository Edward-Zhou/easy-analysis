using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}