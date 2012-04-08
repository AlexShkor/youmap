using System.Linq;
using System.Web.Mvc;
using YouMap.Documents.Services;
using YouMap.Framework;

namespace YouMap.Areas.Mobile.Controllers
{
    public class CheckInsController : YouMap.Controllers.CheckInsController
    {
        public CheckInsController(ImageService imageService, ICommandService commandService, UserDocumentService documentService) : base(imageService, commandService, documentService)
        {
        }

        public ActionResult ForPlace(string placeId)
        {
            var model = _documentService.GetCheckInsGroupsForPlace(placeId, 20).Select(MapToListItem).SelectMany(x => x);
            return View("Index", model);
        }
        public ActionResult ForUser(string userId)
        {
            var user = _documentService.GetById(userId);
            var model = user.CheckIns.OrderByDescending(x=> x.Visited).GroupBy(x=> user).Select(MapToListItem).SelectMany(x => x);
            return View("Index", model);
        }
    }
}