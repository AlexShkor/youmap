using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
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

        public ActionResult Index(MapFilter filter)
        {
            var model = new MapModel();
            model.Places = _documentService.GetAll().Select(Map).ToList();
            if (filter.Latitude.HasValue() && filter.Longitude.HasValue())
            {
                var location = Location.Parse(filter.Latitude, filter.Longitude);
                model.Latitude = location.Latitude;
                model.Longitude = location.Longitude;
                model.ZooomToPlace();
            }
            if (filter.PlaceId.HasValue())
            {
                foreach (var place in model.Places.Where(x => x.Id == filter.PlaceId))
                {
                    place.OpenOnLoad = true;
                }
            }
            var js = new JavaScriptSerializer();
            model.Json = js.Serialize(model);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult CheckNearby(double? latitude, double? longitude)
        {
            if (latitude.HasValue && longitude.HasValue && SessionContext.IsUserAuthorized())
            {
                var location = new Location
                                   {
                                       Latitude = latitude.Value,
                                       Longitude = longitude.Value
                                   };
                SessionContext.UserInfo.Location = location;
                if (false)
                {
                    var command = new User_UpdateMarkCommand
                                      {
                                          UserId = User.Id,
                                          Location = location
                                      };
                    Send(command);
                }
                
            }
            return Result();
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

        private PlaceModel Map(PlaceDocument doc)
        {
            return new PlaceModel
            {
                Id = doc.Id,
                Address = doc.Address,
                Description = doc.Description,
                Icon = _imageService.GetIconModel(doc.CategoryId),
                X = doc.Location.Latitude,
                Y = doc.Location.Longitude,
                Title = doc.Title,
                Shadow = _imageService.IconShadowModel,
                CategoryId = doc.CategoryId,
                InfoWindowUrl = Url.Action("PlaceInfo", "Places", new { id = doc.Id })
            };
        }

        [HttpGet]
        [ActionName("CheckIn")]
        [VkAccess]
        public ActionResult CheckInGet(CheckInModel model)
        {
            model = model ?? new CheckInModel();  
            var filter = new PlaceDocumentFilter {PlaceId = model.PlaceId};
            if (string.IsNullOrEmpty(model.PlaceId) && (model.Latitude.HasValue() && model.Longitude.HasValue()))
            {
                var location = Location.Parse(model.Latitude, model.Longitude);
                filter.Location = location;
            }
            var place = _documentService.GetByFilter(filter).SingleOrDefault();
            if (place != null)
            {
                model.DisplayPlace = true;
                model.PlaceId = place.Id;
                model.CheckInUrl = Url.Action("Index", "Map", new {PlaceId = place.Id});
                model.Latitude = place.Location.GetLatitudeString();
                model.Longitude = place.Location.GetLongitudeString();
                model.LogoUrl = _imageService.GetPlaceLogoUrl(place);
                model.SetMemoWithTemplate(place.Title);
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
            return RespondTo();
        }
    }

    public class MapFilter
    {
        public string CategoryId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string PlaceId { get; set; }
        public string CheckinId { get; set; }
        public string EventId { get; set; }
    }

}
