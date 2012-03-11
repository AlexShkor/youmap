using System.IO;
using System.Linq;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;

namespace YouMap
{
    public class ImageService
    {
        const string ContentUrl = "/content";
        const string UserFiles = "/UserFiles";
        
        
        private string PlaceIconsPath { get { return Path.Combine(UserFiles, "PlaceIcons"); } }

        public string YouIcon { get { return Path.Combine(UserFiles, "Y.png"); } }
        public string IconShadow { get { return Path.Combine(UserFiles, "shadow.png"); } }

        private readonly CategoryDocumentService _categoriesDocumentService;

        public ImageService(CategoryDocumentService categoriesDocumentService)
        {
            _categoriesDocumentService = categoriesDocumentService;
        }

        protected IEnumerable<CategoryDocument> Categories { get { return _categoriesDocumentService.GetAll(); } } 

        public string GetIconForCategory(string categoryId)
        {
            return Path.Combine(PlaceIconsPath,categoryId, Categories.Single(x => x.Id == categoryId).Icon);
        }
    }
}