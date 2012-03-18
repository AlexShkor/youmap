using System;
using System.Linq;
using System.Web.Mvc;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
using YouMap.Framework;
using YouMap.Framework.Environment;
using YouMap.Framework.Extensions;
using YouMap.Models;

namespace YouMap.Controllers
{
    public class VkController : BaseController
    {
        private readonly UserDocumentService _documentService;
        private readonly IIdGenerator _idGenerator;
        private ImageService _imageService;


        public VkController(ICommandService commandService, UserDocumentService documentService, IIdGenerator idGenerator, ImageService imageService)
            : base(commandService)
        {
            _documentService = documentService;
            _idGenerator = idGenerator;
            _imageService = imageService;
        }

        public ActionResult GetUsersLocation(string ids)
        {
            var ar = ids.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            var random = new Random();
            

            var result = ar.Select(x => new FriendMarkerModel
                                            {
                                                Id = x,
                                                X = 53.90234 + (random.NextDouble() - random.NextDouble())/10,
                                                Y = 27.561896 + (random.NextDouble() - random.NextDouble()) / 10,
                                                Visited = (DateTime.Now - DateTime.Now.AddDays(- random.Next(100))).ToDisplayString(),
                                                InfoWindowUrl = Url.Action("UserInfo","Users",new {id=x}),
                                                Shadow = _imageService.FriendShadowModel,
                                            }).Take(5);
            var r = _documentService.GetByFilter(new UserFilter {VkIdIn = ar});
           
            AjaxResponse.AddJsonItem("model", result);
            return Result();
        }

        public bool AddFriendsToDatabase(string ids)
        {
            var ar = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 10; i++)
            {
                var command = new User_CreateCommand
                                  {
                                      UserId = _idGenerator.Generate(),
                                      FirstName = "Name",
                                      LastName = "Surname",
                                      Vk = new VkData()
                                               {
                                                   Id =ar[i]
                                               }
                                  };
                Send(command);
            }
            return true;
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
