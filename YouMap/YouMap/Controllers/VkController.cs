using System;
using System.Linq;
using System.Web.Mvc;
using YouMap.Documents.Services;
using YouMap.Domain.Enums;
using YouMap.Framework;
using YouMap.Framework.Extensions;
using YouMap.Models;

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
                                                Latitude = 53.90234 + (random.NextDouble() - random.NextDouble())/10,
                                                Longitude = 27.561896 + (random.NextDouble() - random.NextDouble()) / 10,
                                                Visited = (DateTime.Now - DateTime.Now.AddDays(- random.Next(100))).ToDisplayString()
                                            }).Take(5);
            var r = _documentService.GetByFilter(new UserFilter {VkIdIn = ar});
            AjaxResponse.AddJsonItem("locations", result);
            return Result();
        }

        public String RemoveAll()
        {
            _documentService.Remove(new UserFilter());
            return "Success";
        }

        public ActionResult Panel()
        {
            var model = new VkPanelModel();
            model.IsVkUser = User != null && User.HasPermissions(UserPermissionEnum.VkUser);
            return PartialView("Panel",model);
        }
    }
}
