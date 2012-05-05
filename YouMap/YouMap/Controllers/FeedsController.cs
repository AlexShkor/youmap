using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Framework;
using YouMap.Models;

namespace YouMap.Controllers
{
    public class FeedsController : BaseController
    {
        private readonly FeedDocumentService _feedDocumentService;
        private readonly UserDocumentService _userDocumentService;

        public FeedsController(ICommandService commandService, FeedDocumentService feedDocumentService, UserDocumentService userDocumentService) : base(commandService)
        {
            _feedDocumentService = feedDocumentService;
            _userDocumentService = userDocumentService;
        }

        public ActionResult Index()
        {
            var model = _feedDocumentService.GetAll().Select(x=> x.Name);
            return View(model);
        }

        [Authorize]
        public ActionResult Create(string name)
        {
            var feed = _feedDocumentService.GetByName(name);
            if (feed == null)
            {
                ValidateModel(name);
                if (name.Length < 2 || name.Length > 100)
                {
                    ModelState.AddModelError("Name", "длина хэштега должна быть от 2 до 100 символов");
                }
                if (!IsValidCharsForFeedName(name))
                {
                    ModelState.AddModelError("Name", "недопустимые символы в имени фида");
                }
                if (ModelState.IsValid)
                {
                    var cmd = new User_CreateFeedCommand
                                  {
                                      UserId = User.Id,
                                      Name = name
                                  };
                    Send(cmd);
                    AjaxResponse.Options.SuccessMessage = "Вы создали фид #" + name;
                    AjaxResponse.ReloadPage = true;
                }
            }
            else
            {
                    var cmd = new User_SubscribeFeedCommand
                                  {
                                      UserId = User.Id,
                                      Feed = name
                                  };
                    Send(cmd);
                    AjaxResponse.Options.SuccessMessage = "Вы подписались на #" + name;
                    AjaxResponse.ReloadPage = true;
                
            }
            return Result();
        }

        private bool IsValidCharsForFeedName(string name)
        {
            return Regex.IsMatch(name, @"[\w]");
        }

        [Authorize]
        public ActionResult Subscribe(string name)
        {
            var feed = _feedDocumentService.GetByName(name);
            if (feed != null)
            {
                var cmd = new User_SubscribeFeedCommand
                              {
                                  UserId = User.Id,
                                  Feed = name
                              };
                Send(cmd);
                AjaxResponse.Options.SuccessMessage = "Вы подписались на #" + name;
            }
            return Result();
        }

        [Authorize]
        public ActionResult Details(string name)
        {
            var model = new FeedDetailsModel();
            model.Name = name;
            var feed = _feedDocumentService.GetByName(name);
            if (feed != null)
            {
                model.Exists = true;
                model.Ownerid = feed.OwnerId;
            }
            var user = _userDocumentService.GetById(User.Id);
            model.IsSubscribed = user.Feeds.Contains(name);
            return View(model);
        }

        public FeedDetailsModel MapDetailsModel(FeedDocument doc)
        {
            return new FeedDetailsModel
                       {
                           Exists = true,
                           Name = doc.Name,
                           Ownerid = doc.OwnerId
                       };
        }


         [Authorize]
        public ActionResult Unsubscribe(string name)
        {
            var cmd = new User_UnsubscribeFeedCommand
            {
                UserId = User.Id,
                Feed = name
            };
            Send(cmd);
            AjaxResponse.Options.SuccessMessage = "Вы отменили подписку на фид #" + name;
            return Result();
        }
    }
}
