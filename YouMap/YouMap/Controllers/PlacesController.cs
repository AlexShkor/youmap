using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using mPower.Framework;

namespace YouMap.Controllers
{
    public class PlacesController : BaseController
    {
        const string contentUrl = "/content/images/place_icons/64x64/";
        private readonly PlaceDocumentService _docimentService;

        public PlacesController(ICommandService commandService, PlaceDocumentService docimentService) : base(commandService)
        {
            _docimentService = docimentService;
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

        private bool Compare(string title, string term)
        {
            title = title.ToLower();
            term = term.ToLower();
            var titleWords = title.Split(new[]{" "}, StringSplitOptions.RemoveEmptyEntries);
            return titleWords.Any(x => x.StartsWith(term));
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
