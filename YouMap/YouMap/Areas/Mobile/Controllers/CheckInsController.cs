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

        
    }
}