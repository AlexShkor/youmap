using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Documents.Services;
using mPower.Framework;

namespace YouMap.Controllers
{
    public class VkController : BaseController
    {
        private readonly UserDocumentService _documentService;

        public VkController(ICommandService commandService, UserDocumentService documentService)
            : base(commandService)
        {
            _documentService = documentService;
        }

        public ActionResult GetUsersLocation(string ids)
        {
            var ar = ids.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            var random = new Random();


            var result = ar.Select(x => new
                                            {
                                                Id = x,
                                                Latitude = 53.90234 + (random.NextDouble() - random.NextDouble())/4,
                                                Longitude = 27.561896 + (random.NextDouble() - random.NextDouble()) / 4,
                                            });
            var r = _documentService.GetByFilter(new UserFilter {VkIdIn = ar});
            AjaxResponse.AddJsonItem("locations", result);
            return Result();
        }

        public String RemoveAll()
        {
            _documentService.Remove(new UserFilter());
            return "Success";
        }
    }
}
