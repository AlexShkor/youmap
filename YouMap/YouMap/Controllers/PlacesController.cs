using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using YouMap.ActionFilters;
using YouMap.Documents.Documents;
using YouMap.Documents.Lucene;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
using YouMap.Framework;
using YouMap.Framework.Environment;
using YouMap.Models;

namespace YouMap.Controllers
{
    public class PlacesController : BaseController
    {
        private const string PlacesDir = "/UserFiles/Places/";

        private readonly PlaceDocumentService _documentService;
        private readonly ImageService _imageService;
        private readonly IIdGenerator _idGenerator;
        private readonly CategoryDocumentService _categoriesDocumentService;
        private readonly UserDocumentService _userDocumentService;
        private readonly PlaceLuceneService _placeLuceneService;
        private IEnumerable<CategoryDocument> _categories;
        public IEnumerable<CategoryDocument> Categories { get { return _categories = _categories ?? _categoriesDocumentService.GetAll(); } } 

        public PlacesController(ICommandService commandService, PlaceDocumentService documentService,ImageService imageService, IIdGenerator idGenerator, CategoryDocumentService categoriesDocumentService, PlaceLuceneService placeLuceneService, UserDocumentService userDocumentService) : base(commandService)
        {
            _documentService = documentService;
            _imageService = imageService;
            _idGenerator = idGenerator;
            _categoriesDocumentService = categoriesDocumentService;
            _placeLuceneService = placeLuceneService;
            _userDocumentService = userDocumentService;
        }

        public ActionResult Index()
        {
            var filter = new PlaceDocumentFilter();         
            filter.StatusNotIn.Add(PlaceStatusEnum.Deleted);
            if (!IsAdmin)
            {
                filter.OwnerId = User.Id;
                filter.StatusNotIn.Add(PlaceStatusEnum.Blocked);
            }
            var docs = _documentService.GetByFilter(filter);
            var model = docs.Select(MapListItem);
            return View(model);
        }

        public ActionResult Search(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var luceneDocs = _placeLuceneService.Search(term);
                if (luceneDocs.Any())
                {
                    var filter = new PlaceDocumentFilter {IdIn = luceneDocs.Select(x => x.Id)};
                    var places =
                        _documentService.GetByFilter(filter).
                            Select(MapForSearch);
                    return Json(places);
                }
            }
            return Json(new { });
        }

        public ActionResult SearchForm()
        {
            return PartialView();
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
            return RespondTo(model);
        }

        [HttpGet]
        public ActionResult PlaceInfo(string id)
        {
            var doc = _documentService.GetById(id);
            var model = Map(doc);
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
                model.Id = _idGenerator.Generate();
                TrySaveImage(model);
                var command = new Place_CreateCommand
                {
                    Id = model.Id,
                    Logo = model.LogoFileName,
                    Location = location,
                    Title = model.Title,
                    Description = model.Description,
                    Address = model.Address,
                    CategoryId = model.CategoryId,
                    WorkDays = model.WorkDays,
                    Status = IsAdmin ? PlaceStatusEnum.Active : PlaceStatusEnum.Hidden
                };
                Send(command);
                AjaxResponse.RedirectUrl = Url.Action("Index");
            }
            return RespondTo(model);
        }

         private object MapForSearch(PlaceDocument doc)
         {
             return new
                        {
                            Id = doc.Id,
                            Address = doc.Address,
                            Description = doc.Description,
                            X = doc.Location.Latitude,
                            Y = doc.Location.Longitude,
                            Title = doc.Title,
                            CategoryId = doc.CategoryId,
                            //TODO: Uncomment when styles will be fixed
                            //Icon = _imageService.GetPlaceLogoUrl(doc)
                        };
         }

        private PlaceInfoModel Map(PlaceDocument doc)
        {
            return new PlaceInfoModel
            {
                Id = doc.Id,
                Description = doc.Description,
                Logo = _imageService.GetPlaceLogoUrl(doc),
                Title = doc.Title,
            };
        }

        private PlaceListItem MapListItem(PlaceDocument doc)
        {
            var model = new PlaceListItem
            {
                Id = doc.Id,
                Address = doc.Address,
                Description = doc.Description,
                Icon = _imageService.GetIconForCategory(doc.CategoryId),
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
            var place = _documentService.GetById(id);
            CheckPermissions(place);
            return RespondTo(MapToEdit(place));
        }

        private AddPlaceModel MapToEdit(PlaceDocument place)
        {
            var model =  new AddPlaceModel
                       {
                           Id = place.Id,
                           Title = place.Title,
                           Description = place.Description,
                           Address = place.Address,
                           CategoryId = place.CategoryId,
                           LogoFileName = place.Logo,
                           WorkDays = place.WorkDays,
                           Latitude = place.Location.GetLatitudeString(),
                           Longitude = place.Location.GetLongitudeString(),
                           Categories = GetCategorySelectList()
                       };
            if (!Request.IsAjaxRequest())
            {
                model.Map = new MapModel();
                model.DisplayMap = true;
            }
            return model;
        }

        [HttpPost]
        public ActionResult Edit(AddPlaceModel model)
        {
            if (ModelState.IsValid)
            {
                var location = Location.Parse(model.Latitude, model.Longitude);
                TrySaveImage(model);
                var command = new Place_UpdateCommand
                                  {
                                      Id = model.Id,
                                      Logo = model.LogoFileName,
                                      Location = location,
                                      Title = model.Title,
                                      Description = model.Description,
                                      Address = model.Address,
                                      CategoryId = model.CategoryId,
                                      WorkDays = model.WorkDays
                                  };
                Send(command);
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
            var place = _documentService.GetById(id);
            CheckPermissions(place);
        }

        [HttpGet]
        [Admin]
        public ActionResult Assign(string id)
        {
            var doc = _documentService.GetById(id);
            var model = MapToAssignModel(doc);
            return RespondTo(model);
        }

        private PlaceAssignModel MapToAssignModel(PlaceDocument doc)
        {
            return new PlaceAssignModel
                       {
                           Address = doc.Address,
                           Id = doc.Id,
                           Title = doc.Title,
                           UserId = doc.OwnerId,
                           Users = new SelectList(_userDocumentService.GetAll().Select(x => new
                                                                                                {
                                                                                                    Text =
                                                                                                x.FullName ?? x.Email,
                                                                                                    Value = x.Id
                                                                                                }), "Value", "Text")
                       };
        }

        [HttpPost]
        [Admin]
        public ActionResult Assing(PlaceAssignModel model)
        {
            if (ModelState.IsValid)
            {
                var command = new Place_AssignCommand
                                  {
                                      OwnerId = model.UserId,
                                      PlaceId = model.Id
                                  };
                Send(command);
                return RedirectToAction("Index");
            }
            return RespondTo(model);
        }

        private void TrySaveImage(AddPlaceModel model)
        {
            try
            {
                model.LogoFileName = SaveImage(model.LogoFile, model.Id);
            }
            catch
            {
                ModelState.AddModelError("Icon", "Не удалось сохранить изображение на сервере.");
            }
        }

        private String SaveImage(HttpPostedFileBase file, string id)
        {
            var image = new WebImage(file.InputStream);
            if (image.Width > 120 || image.Height > 80)
            {
                image = image.Resize(120, 80);
            }
            var filename = String.Format("logo{0}x{1}", 120, 80) + Path.GetExtension(file.FileName);
            image.Save(GetSavePathFor(id, filename));
            return filename;
        }

        private string GetSavePathFor(string id, string filename)
        {
            var dir = Path.Combine(Server.MapPath(PlacesDir), id);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, filename);
        }
    }
}
