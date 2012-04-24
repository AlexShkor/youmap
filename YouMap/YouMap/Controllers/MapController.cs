using System;
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
        private const int MaxCountPerDay = 2;
        private readonly PlaceDocumentService _placeDocumentService;
        private readonly ImageService _imageService;
        private readonly UserDocumentService _userDocumentService;


        public MapController(ICommandService commandService, PlaceDocumentService placeDocumentService, ImageService imageService, UserDocumentService userDocumentService)
            : base(commandService)
        {
            _placeDocumentService = placeDocumentService;
            _imageService = imageService;
            _userDocumentService = userDocumentService;
        }

        public ActionResult Index(MapFilter filter)
        {
            var model = new MapModel();
            model.Defaultlocation = DefaultLocation;
            if (SessionContext.Location != null)
            {
                model.UserLocation = SessionContext.Location;
            }
            model.UserIcon = _imageService.UserIconModel;
            if (filter.Latitude.HasValue() && filter.Longitude.HasValue())
            {
                var location = Location.Parse(filter.Latitude, filter.Longitude);
                model.Latitude = location.Latitude;
                model.Longitude = location.Longitude;
                model.ZooomToPlace();
            }
            return RespondTo(request =>
            {
                request.Html= request.Ajax = () => View(model);
                request.Json = () =>
                {
                    model.Places =_placeDocumentService.GetByFilter(new PlaceDocumentFilter{StatusEq = PlaceStatusEnum.Active}).Select(Map).ToList();
                    if (filter.PlaceId.HasValue())
                    {
                        foreach (var place in model.Places.Where(x => x.Id == filter.PlaceId))
                        {
                            place.OpenOnLoad = true;
                        }
                    }

                    if (filter.EventId.HasValue())
                    {
                        model.OpenPopupUrl = Url.Action("Details", "Events", new{id = filter.EventId});
                    }
                    AjaxResponse.AddJsonItem("model",model);
                    return Result();
                };
            });
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

        protected virtual PlaceModel Map(PlaceDocument doc)
        {
            return new PlaceModel
            {
                Id = doc.Id,
                Address = doc.Address,
                Description = doc.Description,
                Icon = _imageService.GetIconModel(doc.CategoryId),
                X = doc.Location.Latitude,
                Y = doc.Location.Longitude,
                Layer = doc.Layer,
                Title = doc.Title,
                Shadow = _imageService.IconShadowModel,
                CategoryId = doc.CategoryId,
                InfoWindowUrl = Url.Action("PlaceInfo", "Places", new { id = doc.Id })
            };
        }

        [HttpGet]
        [ActionName("CheckIn")]
        public ActionResult CheckInGet(CheckInModel model)
        {
            model = model ?? new CheckInModel();  
            var filter = new PlaceDocumentFilter {PlaceId = model.PlaceId};
            if (string.IsNullOrEmpty(model.PlaceId))
            {
                if ((model.Latitude.HasValue() && model.Longitude.HasValue()))
                {
                    var location = Location.Parse(model.Latitude, model.Longitude);
                    filter.Location = location;
                }
                else
                {
                    var location = (SessionContext.Location ?? DefaultLocation);
                    model.Latitude = location.GetLatitudeString();
                    model.Longitude = location.GetLongitudeString();
                    filter.Location = location;
                }
            }
            var place = _placeDocumentService.GetByFilter(filter).SingleOrDefault();
            if (place != null)
            {
                var user = _userDocumentService.GetById(User.Id);
                model.LeftCount = MaxCountPerDay - user.CheckIns.Count(x => x.PlaceId == model.PlaceId && x.Visited.Date == DateTime.Now.Date); 
                model.Limited = true;
                model.DisplayPlace = true;
                model.PlaceId = place.Id;
                model.CheckInUrl = Url.Action("Index", "Map", new {PlaceId = place.Id, area=""});
                model.Latitude = place.Location.GetLatitudeString();
                model.Longitude = place.Location.GetLongitudeString();
                model.LogoUrl = _imageService.GetPlaceLogoUrl(place);
                model.SetMemoWithTemplate(place.Title);
            }
            return RespondTo(model);
        }

        [HttpPost]
        public ActionResult CheckIn(CheckInModel model)
        {
            var user = _userDocumentService.GetById(User.Id);
            model.LeftCount = MaxCountPerDay -
                              user.CheckIns.Count(x => x.PlaceId == model.PlaceId && x.Visited.Date == DateTime.Now.Date);
            if (model.PlaceId.HasValue() && model.LeftCount <= 0)
            {
                ModelState.AddModelError("Error", "Сегодня вы уже не можете отметиться в этом месте.");
            }
            if (!ModelState.IsValid)
            {
                return CheckInGet(model);
            }
            Location location = Location.TryParse(model.Latitude, model.Longitude) ?? SessionContext.Location;
            var command = new User_AddCheckInCommand
                              {
                                  Memo = model.Memo,
                                  Title = model.Memo.Ellipsize(50),
                                  Location = location,
                                  PlaceId = model.PlaceId,
                                  UserId = User.Id
                              };
            Send(command);
            if (model.Share)
            {
                ShareCheckIn(model);
            }
            AjaxResponse.ClosePopup = true;
            AjaxResponse.RedirectUrl = model.RedirectUrl;
            return RespondTo(request =>
                                 {
                                     request.Ajax = PartialView;
                                     request.Json = Result;
                                     request.Html = () => Redirect(model.RedirectUrl ?? Url.Action("Index"));
                                 });
        }

        protected virtual void ShareCheckIn(CheckInModel model)
        {
            
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
