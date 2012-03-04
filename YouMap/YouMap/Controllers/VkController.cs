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
    }
}
