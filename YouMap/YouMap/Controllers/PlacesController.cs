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
using SoundInTheory.DynamicImage.Fluent;
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
                var luceneDocs = _placeLuceneService.Search(term,PlaceStatusEnum.Active);
                if (luceneDocs.Any())
                {
                    var filter = new PlaceDocumentFilter {IdIn = luceneDocs.Select(x => x.Id )};
                    var places =
                        _documentService.GetByFilter(filter).
                            Select(MapForSearch);
                    AjaxResponse.AddJsonItem("places",places);
                }
            }
            return Result();
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
                    Layer = model.Layer,
                    Tags = model.Tags,
                    WorkDays = model.WorkDays,
                    Status = IsAdmin ? PlaceStatusEnum.Active : PlaceStatusEnum.Hidden
                };
                Send(command);
                return RespondTo(r =>
                                     {
                                         var url = Url.Action("Index", "Map", new {placeId = model.Id});
                                         AjaxResponse.RedirectUrl = url;
                                         r.Html = () => Redirect(url);
                                         r.Json = Result;
                                     });
            }
            return RespondTo(model);
        }

        public ActionResult ChangeLayer(string id, int layer)
        {
            var command =new Place_ChangeLayerCommand
            {
                PlaceId = id,
                Layer = layer
            };
            Send(command);
            AjaxResponse.Options.SuccessMessage = String.Format("Слой изменен на {0}",layer);
            return RespondTo(r =>
                                 {
                                     r.Json = Result;
                                     r.Html = () => RedirectToAction("Index");
                                 });
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
                MapUrl = Url.Action("Index","Map", new {placeId = doc.Id}),
                Tags =  doc.Tags,
                HideAction = doc.Status == PlaceStatusEnum.Hidden ? "Activate" : "Hide",
                HideLabel = doc.Status == PlaceStatusEnum.Hidden ? "Активировать" : "Спрятать",
                DisplayBlockAction = IsAdmin && doc.Status != PlaceStatusEnum.Blocked,
                Layer = doc.Layer        
            };
            return model;
        }

        [HttpPost]
        public ActionResult EditTags(string placeId, string tags)
        {
            var arr = tags.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            var place = _documentService.GetById(placeId);
            var command = new Place_UpdateCommand()
                              {
                                  Id = place.Id,
                                  Logo = place.Logo,
                                  Location = place.Location,
                                  Title = place.Title,
                                  Layer = place.Layer,
                                  Description = place.Description,
                                  Address = place.Address,
                                  CategoryId = place.CategoryId,
                                  WorkDays = place.WorkDays,
                                  Tags = arr.ToList()
                              };
            Send(command);
            AjaxResponse.Options.SuccessMessage = String.Format("{0} изменен.", place.Title);
            return Result();
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
                           Layer = place.Layer,
                           Categories = GetCategorySelectList(),
                           Tags = place.Tags
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
                                      Layer = model.Layer,
                                      Description = model.Description,
                                      Address = model.Address,
                                      CategoryId = model.CategoryId,
                                      WorkDays = model.WorkDays,
                                      Tags  = model.Tags
                                  };
                Send(command);
                return RespondTo(r =>
                                     {
                                         var url = Url.Action("Index", "Map", new {placeId = model.Id});
                                         AjaxResponse.RedirectUrl = url;
                                         r.Html = () => Redirect(url);
                                         r.Json = Result;
                                     });   
            }
            return RespondTo(model);
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

        public ActionResult Near(string x, string y)
        {
            var location = Location.Parse(x, y);
            var places = _documentService.GetPlacesForLocation(location);
            var model = places.Select(MapSelectList);
            return RespondTo(r =>
                          {
                              r.Ajax = () => PartialView("SelectList", model);
                              r.Html = () => View("SelectList", model);
                              r.Json = () =>
                                           {
                                               if (model.Any())
                                               {
                                                   AjaxResponse.Render("#placeSelectList", "SelectList", model);
                                               }
                                               else
                                               {
                                                   AjaxResponse.AddUpdateItem("#placeHolder","");
                                               }
                                               return Result();
                                           };
                          });
        }

        private PlaceSelectListItem MapSelectList(KeyValuePair<double,PlaceDocument> doc)
        {
            return MapSelectList(doc.Value);
        }

        private PlaceSelectListItem MapSelectList(PlaceDocument doc)
        {
            return new PlaceSelectListItem
                       {
                           Text = doc.Title,
                           Value = doc.Id,
                           Url = Url.Action("Index","Map",new {placeId = doc.Id})
                       };
        }

        private void TrySaveImage(AddPlaceModel model)
        {
            try
            {
                model.LogoFileName = SaveImage(model.LogoFile, model.Id);
            }
            catch
            {
                //ModelState.AddModelError("Icon", "Не удалось сохранить изображение на сервере.");
            }
        }

        private String SaveImage(HttpPostedFileBase file, string id)
        {
            var filename = String.Format("logo{0}x{1}", 120, 80) + Path.GetExtension(file.FileName);
            var memoryStream = new MemoryStream();
            file.InputStream.CopyTo(memoryStream);
            var layer = LayerBuilder.Image.SourceBytes(memoryStream.ToArray())
                .WithFilter(FilterBuilder.Resize.To(120, 80)).ToLayer();
            layer.Process();
            layer.Bitmap.Save(GetSavePathFor(id, filename));
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

    public class PlaceSelectListItem: SelectListItem
    {
        public string Url { get; set; }
    }
}
