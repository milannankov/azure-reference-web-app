using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bookstore.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var executionEnvironment = ConfigurationManager.AppSettings["ExecutionEnvironment"];

            ViewBag.ExecutionEnvironment = executionEnvironment;

            return View();
        }
    }
}