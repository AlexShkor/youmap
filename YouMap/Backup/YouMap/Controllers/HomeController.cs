using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YouMap.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Map(MapModel model)
        {
            model = model ?? new MapModel();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }
    }
}

