using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mPower.Framework;

namespace YouMap.Controllers
{
    public class VkController : BaseController
    {
        public VkController(ICommandService commandService) : base(commandService)
        {
        }

        public ActionResult GetUsersLocation(string ids)
        {
            var ar = ids.Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries);
            var random = new Random();
            var result = ar.Select(x => new
                                             {
                                                 Id = x,
                                                 Latitude = 53.90234 + random.NextDouble() - random.NextDouble(),
                                                 Longitude = 27.561896 + random.NextDouble() - random.NextDouble(),
                                             });
            AjaxResponse.AddJsonItem("locations",result);
            return Result();
        }
    }
}
