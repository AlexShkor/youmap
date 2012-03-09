using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
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
    public class PlacesController : BaseController
    {
        const string contentUrl = "/content/images/place_icons/64x64/";
        private readonly PlaceDocumentService _docimentService;
        private readonly IIdGenerator _idGenerator;
        public PlacesController(ICommandService commandService, PlaceDocumentService docimentService, IIdGenerator idGenerator) : base(commandService)
        {
            _docimentService = docimentService;
            _idGenerator = idGenerator;
        }

        public ActionResult Search(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new {});
            }
            var places = _docimentService.GetAll().Where(x => Compare(x.Title,term)).Select(Map);
            return Json(places);
        }

        public ActionResult SearchForm()
        {
            return PartialView();
        }

        private bool Compare(string title, string term)
        {
            title = title.ToLower();
            term = term.ToLower();
            var titleWords = title.Split(new[]{" "}, StringSplitOptions.RemoveEmptyEntries);
            return titleWords.Any(x => x.StartsWith(term));
        }


        [HttpGet]
        [Authorize]
        [Role(UserPermissionEnum.Admin, UserPermissionEnum.Advertiser)]
        public ActionResult AddPlace()
        {
            var model = new AddPlaceModel();
            if (!Request.IsAjaxRequest())
            {
                model.Map = new MapModel();
                model.DisplayMap = true;
            }
            AjaxResponse.Render(".control-content", "AddPlace", model);
            return RespondTo(model);
        }

        [HttpPost]
        [Authorize]
        [Role(UserPermissionEnum.Admin, UserPermissionEnum.Advertiser)]
        public ActionResult AddPlace(AddPlaceModel model)
        {
            if (ModelState.IsValid)
            {
                var location = Location.Parse(model.Latitude, model.Longitude);
                var command = new Place_CreateCommand()
                {
                    Id = _idGenerator.Generate(),
                    Icon = Path.GetFileName(model.Icon),
                    Location = location,
                    Title = model.Title,
                    Description = model.Description,
                    Address = model.Address
                };
                Send(command);
            }
            AjaxResponse.Render(".control-content", "AddPlace", model);
            return RespondTo(model);
        }

        private PlaceModel Map(PlaceDocument doc)
        {
            return new PlaceModel
            {
                Id = doc.Id,
                Address = doc.Address,
                Description = doc.Description,
                Icon = Path.Combine(contentUrl.Replace("64x64", "24x24"), doc.Icon),
                Latitude = doc.Location.Latitude,
                Longitude = doc.Location.Longitude,
                Title = doc.Title
            };
        }

    }
}
