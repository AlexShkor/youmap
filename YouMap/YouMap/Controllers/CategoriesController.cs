using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Scripts;
using mPower.Framework;
using mPower.Framework.Environment;

namespace YouMap.Controllers
{
    public class CategoriesController : BaseController
    {
        private IIdGenerator _idGenerator;
        private CategoryDocumentService _documentService;
        //
        // GET: /Categories/

        public const int IconWidth = 24;
        public const int IconHeight = 24;
        private const string PlaceIconsDir = "/UserFiles/PlaceIcons/";

        public CategoriesController(ICommandService commandService, IIdGenerator idGenerator, CategoryDocumentService categoryDocumentService) : base(commandService)
        {
            _idGenerator = idGenerator;
            _documentService = categoryDocumentService;
        }

        public ActionResult Index()
        {
            var model = _documentService.GetAll().Select(Map);
            AjaxResponse.Render(".control-content", "Index", model);
            return Result();
        }

        private CategoryModel Map(CategoryDocument doc)
        {
            return new CategoryModel
                       {
                           Id = doc.Id,
                           Name = doc.Name,
                           IsTop = doc.IsTop,
                           Icon = Url.Content(Path.Combine(PlaceIconsDir,doc.Id + "/", doc.Icon))
                       };
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            var model = new AddCategoryModel();
            AjaxResponse.Render(".add-category-container", "CreateEditCategory", model);
            return Result();
        }

        public ActionResult Delete(string id)
        {
            var command = new Category_DeleteCommand
                              {
                                  Id = id
                              };
            Send(command);
            return Result();
        }

        public ActionResult Edit(string id)
        {
            var doc = _documentService.GetById(id);
            var model = MapToEditModel(doc);
            return PartialView("CreateEditCategory",model);
        }

        public ActionResult Edit(AddCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                TrySaveImage(model);
                var command = new Category_UpdateCommand
                {
                    Id = model.Id,
                    Name = model.Name,
                    Icon = model.FileName,
                    IsTop = model.IsTop
                };
                Send(command);
            }
            return Result();
        }

        private AddCategoryModel MapToEditModel(CategoryDocument doc)
        {
            return new AddCategoryModel
                       {
                           Id = doc.Id,
                           FileName = doc.Icon,
                           Name = doc.Name,
                           IsTop = doc.IsTop
                       };
        }

        [HttpPost]
        public ActionResult AddCategory(AddCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = _idGenerator.Generate();
                TrySaveImage(model);
                var command = new Category_CreateCommand
                                  {
                                      Id = model.Id,
                                      Name = model.Name,
                                      Icon = model.FileName,
                                      IsTop = model.IsTop
                                  };
                Send(command);
            }
            return Result();
        }

        private void TrySaveImage(AddCategoryModel model)
        {
            try
            {
                var image = new WebImage(model.Icon.InputStream);
                if (image.Width != IconWidth || image.Height != IconHeight)
                {
                    image = image.Resize(IconWidth, IconHeight);
                }
                var filename = IconWidth.ToString() + Path.GetExtension(model.Icon.FileName);
                image.Save(GetSavePathFor(model.Id, filename));
                model.FileName = filename;
            }
            catch
            {
                ModelState.AddModelError("Icon", "Не удалось сохранить изображение на сервере.");
            }
        }

        private string GetSavePathFor(string id, string filename)
        {
            var dir = Path.Combine(Server.MapPath(PlaceIconsDir), id);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir,filename);
        }
    }
}
