using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Models;
using mPower.Framework;
using mPower.Framework.Environment;

namespace YouMap.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly IIdGenerator _idGenerator;
        private readonly CategoryDocumentService _documentService;
        //
        // GET: /Categories/
        private Point IconSmallSize
        {
            get { return new Point(20, 34); }
        }

        private Point IconLargeSize
        {
            get { return new Point(24, 24); }
        }

        private const string PlaceIconsDir = "/UserFiles/PlaceIcons/";

        public CategoriesController(ICommandService commandService, IIdGenerator idGenerator,
                                    CategoryDocumentService categoryDocumentService)
            : base(commandService)
        {
            _idGenerator = idGenerator;
            _documentService = categoryDocumentService;
        }

        public ActionResult Index()
        {
            var model = _documentService.GetAll().Select(Map);
            AjaxResponse.Render(".control-content", "Index", model);
            return RespondTo(model);
        }

        private CategoryModel Map(CategoryDocument doc)
        {
            return new CategoryModel
                       {
                           Id = doc.Id,
                           Name = doc.Name,
                           IsTop = doc.IsTop,
                           Icon = Url.Content(Path.Combine(PlaceIconsDir, doc.Id + "/", doc.Icon))
                       };
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            var model = new AddCategoryModel();
            RenderEditCategory(model);
            return RespondTo(model, "CreateEditCategory");
        }

        public ActionResult Delete(string id)
        {
            var command = new Category_DeleteCommand
                              {
                                  Id = id
                              };
            Send(command);
            return RedirectToAction("Index");
        }

      

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var doc = _documentService.GetById(id);
            var model = MapToEditModel(doc);
            RenderEditCategory(model);
            return RespondTo(model,"CreateEditCategory");
        }

        private void RenderEditCategory(AddCategoryModel model)
        {
            AjaxResponse.Render(".add-category-container", "CreateEditCategory", model);
        }

        [HttpPost]
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
            AjaxResponse.RedirectUrl = Url.Action("Index");
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
            AjaxResponse.RedirectUrl = Url.Action("Index");
            return Result();
        }

        private void TrySaveImage(AddCategoryModel model)
        {
            try
            {
                model.FileName = SaveImageAndGetFilename(model.Icon, model.Id);
            }
            catch
            {
                ModelState.AddModelError("Icon", "Не удалось сохранить изображение на сервере.");
            }
        }

        private String SaveImageAndGetFilename(HttpPostedFileBase file, string id)
        {
            var image = new WebImage(file.InputStream);
            if (image.Width != IconSmallSize.X || image.Height != IconSmallSize.Y)
            {
                image = image.Resize(IconSmallSize.X, IconSmallSize.Y);
            }
            var filename = String.Format("{0}x{1}", IconSmallSize.X, IconSmallSize.Y) + Path.GetExtension(file.FileName);
            image.Save(GetSavePathFor(id, filename));
            return filename;
        }

        private string GetSavePathFor(string id, string filename)
        {
            var dir = Path.Combine(Server.MapPath(PlaceIconsDir), id);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, filename);
        }

        public ActionResult Navigation()
        {
            var model = _documentService.GetByFilter(new CategoryFilter
                                                         {
                                                             IsTop = true
                                                         }).Select(Map);
            return PartialView(model);
        }
    }

}
