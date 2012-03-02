using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
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

        [HttpGet]
        [Authorize]
        public ActionResult AddPlace()
        {
            var model = new AddPlaceModel();
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

        [HttpGet]
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
    }
}
