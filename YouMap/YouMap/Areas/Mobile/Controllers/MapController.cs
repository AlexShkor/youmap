using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Controllers;
using YouMap.Framework;

namespace YouMap.Areas.Mobile.Controllers
{
    public class MapController : BaseController
    {
        //
        // GET: /Mobile/Map/

        public MapController(ICommandService commandService) : base(commandService)
        {
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Settings()
        {
            return View();
        }
    }
}
