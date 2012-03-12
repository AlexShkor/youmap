using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using YouMap.ActionFilters;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
using YouMap.Framework;
using YouMap.Framework.Utils.Extensions;
using YouMap.Models;

namespace YouMap.Controllers
{
    public class MapController : BaseController
    {
        private readonly PlaceDocumentService _documentService;
        private readonly ImageService _imageService;


        public MapController(ICommandService commandService, PlaceDocumentService documentService, ImageService imageService)
            : base(commandService)
        {
            _documentService = documentService;
            _imageService = imageService;
        }

        public ActionResult Index()
        {
            var model = new MapModel();
            model.IconShadow = _imageService.IconShadowModel;
            model.Markers = _documentService.GetAll().Select(Map);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        private PlaceModel Map(PlaceDocument doc)
        {
            return new PlaceModel
                       {
                           Id = doc.Id,
                           Address = doc.Address,
                           Description = doc.Description,
                           Icon = _imageService.GetIconModel(doc.CategoryId),
                           Latitude = doc.Location.Latitude,
                           Longitude = doc.Location.Longitude,
                           Title = doc.Title,
                           InfoWindowUrl = Url.Action("PlaceInfo",new{id=doc.Id})
                       };
        }

        
        public ActionResult CheckNearby(double? latitude, double? longitude)
        {
            if (latitude.HasValue && longitude.HasValue && LastLocation == null && SessionContext.IsUserAuthorized())
            {
                var location = new Location
                                   {
                                       Latitude = latitude.Value,
                                       Longitude = longitude.Value
                                   };
                LastLocation = location;
                var command = new User_UpdateMarkCommand
                                  {
                                      UserId = User.Id,
                                      Location = location
                                  };
                Send(command);
            }
            return Result();
        }

        protected Location LastLocation
        {
            get { return Session["LastLocation"] as Location; }
            set { Session["LastLocation"] = value; }
        }

        

        public ActionResult ControlPanel()
        {
            var model = new ControlPanelModel();
            if (User != null)
            {
                model.IsAdminPanelVisible = IsAdmin;
                model.IsAdvertiserPanelVisible = IsAdvertiser || IsAdmin;
            }
            return PartialView(model);
        }

       

        public ActionResult Filter(MapFilter filter)
        {
            var places = _documentService.GetByFilter(new PlaceDocumentFilter
                                                         {
                                                             CategoryId = filter.CategoryId
                                                         }).Select(Map);
            var model = new MapModel();
            model.IconShadow = _imageService.IconShadowModel;
            model.Markers = places;

            return RespondTo(r =>
            {
                r.Ajax = r.Json = () =>
                {
                    AjaxResponse.Render("#mapHolder", "Index", model);
                    return Result();
                };
                r.Html = () => View("Index",model);
            });
        }

        [HttpGet]
        public ActionResult PlaceInfo(string id)
        {
            var doc = _documentService.GetById(id);
            return RespondTo(Map(doc));
        }

        [HttpGet]
        public ActionResult CheckIn(double? latitude, double? longitude)
        {
            var model = new CheckInModel();
            if (latitude.HasValue && longitude.HasValue)
            {
                var location = new Location(latitude.Value,longitude.Value);
                var place = _documentService.GetByFilter(new PlaceDocumentFilter { Location = location}).SingleOrDefault();
                if (place != null)
                {
                    model.DisplayPlace = true;
                    model.PlaceId = place.Id;
                    model.Memo = place.Title;
                }
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult CheckIn(CheckInModel model)
        {
            if (ModelState.IsValid)
            {
                var location = Location.Parse(model.Latitude, model.Longitude);
                var command = new User_AddCheckInCommand
                                  {
                                      Memo = model.Memo,
                                      Title = model.Memo.Ellipsize(50),
                                      Location =  location,
                                      PlaceId = model.PlaceId,
                                      UserId = User.Id
                                  };
                Send(command);
                AjaxResponse.ClosePopup = true;
            }
            return Result();
        }
    }

    public class MapFilter
    {
        public string CategoryId { get; set; }
    }

}
