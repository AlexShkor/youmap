using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.ActionFilters;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
using YouMap.Models;
using mPower.Framework;
using mPower.Framework.Environment;

namespace YouMap.Controllers
{
    public class MapController : BaseController
    {
        const string contentUrl = "/content/images/place_icons/64x64/";

        private readonly IIdGenerator _idGenerator;
        private readonly PlaceDocumentService _documentService;


        public MapController(IIdGenerator idGenerator, ICommandService commandService, PlaceDocumentService documentService)
            : base(commandService)
        {
            _idGenerator = idGenerator;
            _documentService = documentService;

        }

        public ActionResult Index()
        {
            var model = new MapModel();
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
                           Icon = Path.Combine(contentUrl.Replace("64x64", "24x24"), doc.Icon),
                           Latitude = doc.Latitude,
                           Longitude = doc.Longitude,
                           Title = doc.Title
                       };
        }

        public ActionResult CheckNearby(Location? location)
        {
            if (location.HasValue)
            {
                
            }
            return Result();
        }

        [HttpGet]
        [Authorize]
        [Role(UserPermissionEnum.Admin,UserPermissionEnum.Advertiser)]
        public ActionResult AddPlace()
        {
            var model = new AddPlaceModel();
            if (!Request.IsAjaxRequest())
            {
                model.Map = new MapModel();
                model.Map.Width = 600;
                model.Map.Height = 600;
                model.DisplayMap = true;
            }
            return GetAddPlaceResponse(model);
        }

        private ActionResult GetAddPlaceResponse(AddPlaceModel model)
        {
            return RespondTo
                (request =>
                     {
                         request.Ajax = request.Json = () =>
                                                           {
                                                               AjaxResponse.Render(".control-content", "AddPlace", model);
                                                               return Result();
                                                           };
                         request.Html = () => View(model);
                     });
        }

        [HttpPost]
        [Authorize]
        [Role(UserPermissionEnum.Admin, UserPermissionEnum.Advertiser)]
        public ActionResult AddPlace(AddPlaceModel model)
        {
            if (ModelState.IsValid)
            {
                var command = new Place_CreateCommand()
                        {
                            Id = _idGenerator.Generate(),
                            Icon = Path.GetFileName(model.Icon),
                            Latitude = double.Parse(model.Latitude,CultureInfo.InvariantCulture),
                            Longitude = double.Parse(model.Longitude, CultureInfo.InvariantCulture),
                            Title = model.Title,
                            Description = model.Description,
                            Address = model.Address
                        };
                Send(command);
                RedirectToAction("ControlPanel");
            }
            return GetAddPlaceResponse(model);
        }

        [HttpGet]
        [Admin]
        [Authorize]
        public ActionResult ControlPanel()
        {
            return RespondTo(request =>
                {
                    request.Ajax = () => PartialView("ControlPanelPartial");
                    request.Html = () =>
                            {
                                var model = new MapModel();
                                model.Markers = _documentService.GetAll().Select(Map);
                                return View(model);
                            };
                });
        }

        public ActionResult Filter(MapFilter filter)
        {
            var places = _documentService.GetByFilter(new PlaceDocumentFilter
                                                         {
                                                             CategoryId = filter.CategoryId
                                                         }).Select(Map);
            var model = new MapModel();
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
        [Role(UserPermissionEnum.Admin, UserPermissionEnum.Advertiser)]
        [Authorize]
        public ActionResult PlaceIcons()
        {          
            var model = Directory.GetFiles(Server.MapPath(contentUrl)).Select(x => Path.Combine(contentUrl,Path.GetFileName(x)));
            return PartialView(model);
        }

        public ActionResult PlaceInfo(string id)
        {
            var doc = _documentService.GetById(id);
            return View(Map(doc));
        }

        [HttpGet]
        public ActionResult CheckIn()
        {
            var model = new CheckInModel();
            return RespondTo(request =>
                                 {
                                     request.Ajax = () => PartialView(model);
                                     request.Html = () => View(model);
                                 });
        }
        [HttpPost]
        public ActionResult CheckIn(CheckInModel model)
        {
            if (ModelState.IsValid)
            {
                
            }
            return Result();
        }
    }

    public class MapFilter
    {
        public string CategoryId { get; set; }
    }

}
