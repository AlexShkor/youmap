using System.Web;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Enums;
using YouMap.Models;

namespace YouMap.Factories.Models
{
    //public class PlaceModelFactory
    //{
    //    private readonly PlaceDocumentService _documentService;
    //    protected UrlHelper Url { get; set; }

    //    public PlaceModelFactory(PlaceDocumentService documentService)
    //    {
    //        _documentService = documentService;
    //        Url = new UrlHelper(HttpContext.Current.Request.RequestContext);
    //    }

    //    private object MapForSearch(PlaceDocument doc)
    //    {
    //        return new
    //        {
    //            Id = doc.Id,
    //            Address = doc.Address,
    //            Description = doc.Description,
    //            X = doc.Location.Latitude,
    //            Y = doc.Location.Longitude,
    //            Title = doc.Title,
    //            CategoryId = doc.CategoryId,
    //        };
    //    }

    //    private PlaceModel Map(PlaceDocument doc)
    //    {
    //        return new PlaceModel
    //        {
    //            Id = doc.Id,
    //            Address = doc.Address,
    //            Description = doc.Description,
    //            Icon = _imageService.GetIconModel(doc.CategoryId),
    //            X = doc.Location.Latitude,
    //            Y = doc.Location.Longitude,
    //            Title = doc.Title,
    //            Shadow = _imageService.IconShadowModel,
    //            CategoryId = doc.CategoryId,
    //            InfoWindowUrl = Url.Action("PlaceInfo", "Places", new { id = doc.Id })
    //        };
    //    }

    //    private PlaceListItem MapListItem(PlaceDocument doc)
    //    {
    //        var model = new PlaceListItem
    //        {
    //            Id = doc.Id,
    //            Address = doc.Address,
    //            Description = doc.Description,
    //            Icon = _imageService.GetIconForCategory(doc.CategoryId),
    //            Title = doc.Title,
    //            HideAction = doc.Status == PlaceStatusEnum.Hidden ? "Activate" : "Hide",
    //            HideLabel = doc.Status == PlaceStatusEnum.Hidden ? "Активировать" : "Спрятать",
    //            DisplayBlockAction = IsAdmin && doc.Status != PlaceStatusEnum.Blocked

    //        };
    //        return model;
    //    }

    //}
}