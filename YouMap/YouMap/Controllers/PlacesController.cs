using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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
        private readonly CategoryDocumentService _categoriesDocumentService;

        private IEnumerable<CategoryDocument> _categories;
        public IEnumerable<CategoryDocument> Categories { get { return _categories = _categories ?? _categoriesDocumentService.GetAll(); } } 

        public PlacesController(ICommandService commandService, PlaceDocumentService docimentService, IIdGenerator idGenerator, CategoryDocumentService categoriesDocumentService) : base(commandService)
        {
            _docimentService = docimentService;
            _idGenerator = idGenerator;
            _categoriesDocumentService = categoriesDocumentService;
        }

        public ActionResult Index()
        {
            var filter = new PlaceDocumentFilter();
            
            if (!IsAdmin)
            {
                filter.OwnerId = User.Id;
                filter.StatusNotEqual = PlaceStatusEnum.Blocked;
            }
            var docs = _docimentService.GetByFilter(filter);
            var model = docs.Select(MapListItem);
            return View(model);
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
            model.Categories = GetCategorySelectList();
            if (!Request.IsAjaxRequest())
            {
                model.Map = new MapModel();
                model.DisplayMap = true;
            }
            AjaxResponse.Render(".control-content", "AddPlace", model);
            return RespondTo(model);
        }

        private IEnumerable<SelectListItem> GetCategorySelectList()
        {
            return new SelectList(_categoriesDocumentService.GetAll(), "Id", "Name");
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
                    //Icon = Path.GetFileName(model.Icon),
                    Location = location,
                    Title = model.Title,
                    Description = model.Description,
                    Address = model.Address,
                    CategoryId = model.CategoryId,
                    WorkDays = model.WorkDays
                };
                Send(command);
                AjaxResponse.RedirectUrl = Url.Action("Index");
            }
            return RespondTo(model);
        }

        private PlaceModel Map(PlaceDocument doc)
        {
            return new PlaceModel
            {
                Id = doc.Id,
                Address = doc.Address,
                Description = doc.Description,
                Icon = Path.Combine(contentUrl.Replace("64x64", "24x24"), Categories.Single(x=> x.Id == doc.CategoryId).Icon),
                Latitude = doc.Location.Latitude,
                Longitude = doc.Location.Longitude,
                Title = doc.Title
            };
        }

        private PlaceListItem MapListItem(PlaceDocument doc)
        {
            var model = new PlaceListItem
            {
                Id = doc.Id,
                Address = doc.Address,
                Description = doc.Description,
                Icon = Path.Combine(contentUrl.Replace("64x64", "24x24"), Categories.Single(x=> x.Id == doc.CategoryId).Icon),
                Title = doc.Title,
                HideAction = doc.Status == PlaceStatusEnum.Hidden ? "Activate" : "Hide",
                HideLabel = doc.Status == PlaceStatusEnum.Hidden ? "Активировать" : "Спрятать",
                DisplayBlockAction = IsAdmin && doc.Status != PlaceStatusEnum.Blocked
               
            };
            return model;
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            CheckPermissions(id);
            var command = new Place_ChangeStatusCommand
                              {
                                  PlaceId = id,
                                  Status = PlaceStatusEnum.Deleted
                              };
            Send(command);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Hide(string id)
        {
            CheckPermissions(id);
            return ChangeStatus(id, PlaceStatusEnum.Hidden);
        }

        [HttpGet]
        public ActionResult Activate(string id)
        {
            CheckPermissions(id);
            return ChangeStatus(id, PlaceStatusEnum.Active);
        }

        [HttpGet]
        [Admin]
        public ActionResult Block(string id)
        {
            return ChangeStatus(id, PlaceStatusEnum.Blocked);
        }
     
        private ActionResult ChangeStatus(string id, PlaceStatusEnum status)
        {
            var command = new Place_ChangeStatusCommand
            {
                PlaceId = id,
                Status = status
            };
            Send(command);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var place = _docimentService.GetById(id);
            CheckPermissions(place);
            return RespondTo(place);
        }

        [HttpPost]
        public ActionResult Edit(PlaceEditModel model)
        {
            if (ModelState.IsValid)
            {
                throw new NotImplementedException();
            }
            return Result();
        }

        private void CheckPermissions(PlaceDocument place)
        {
            if(!IsAdmin && place.Status == PlaceStatusEnum.Blocked)
            {
                throw new SecurityException("You don't have persimission for this action.");
            }
            if (!IsAdmin && place.OwnerId != User.Id)
            {
                throw new SecurityException("You don't have persimission for this action.");
            }
        }

        private void CheckPermissions(string id)
        {
            var place = _docimentService.GetById(id);
            CheckPermissions(place);
        }
    }
}
