using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Framework.Utils.Extensions;
using YouMap.Models;

namespace YouMap
{
    public class ImageService
    {
        const string ContentUrl = "/content";
        const string UserFiles = "/UserFiles";
        
        
        private string PlaceIconsPath { get { return Path.Combine(UserFiles, "PlaceIcons"); } }

        private string YouIcon { get { return Path.Combine(UserFiles, "Y.png"); } }
        private string IconShadow { get { return Path.Combine(UserFiles, "shadow.png"); } }
        private string EventIconPath { get { return Path.Combine(UserFiles, "event.png"); } }
        private string CheckinIconPath { get { return Path.Combine(UserFiles, "checkin.png"); } }

        private readonly CategoryDocumentService _categoriesDocumentService;

        public ImageService(CategoryDocumentService categoriesDocumentService)
        {
            _categoriesDocumentService = categoriesDocumentService;
        }

        protected IEnumerable<CategoryDocument> Categories { get { return _categoriesDocumentService.GetAll(); } } 

        public string GetIconForCategory(string categoryId)
        {
            return
                Path.Combine(PlaceIconsPath, categoryId, Categories.Single(x => x.Id == categoryId).Icon).Replace("\\",
                                                                                                                  "/");
        }

        private static HttpContextWrapper HttpContext
        {
            get { return new HttpContextWrapper(System.Web.HttpContext.Current); }
        }

        public MarkerIcon IconShadowModel
        {
            get
            {
                return new MarkerIcon()
                {
                    Path = IconShadow.Replace("\\","/"),
                    Anchor = new Point(0, 34),
                    Point = Point.Empty,
                    Size = new Size(28, 34)
                    
                };
            }
        }

        public MarkerIcon EventIconModel
        {
            get
            {
                return new MarkerIcon()
                {
                    Path = EventIconPath.Replace("\\", "/"),
                    Anchor = new Point(30, 0),
                    Point = Point.Empty,
                    Size = new Size(30, 30)

                };
            }
        }

        public MarkerIcon CheckInIconModel
        {
            get
            {
                return new MarkerIcon()
                {
                    Path = CheckinIconPath.Replace("\\", "/"),
                    Anchor = new Point(0, 0),
                    Point = Point.Empty,
                    Size = new Size(30, 30)

                };
            }
        }

        public MarkerIcon GetIconModel(string categoryId)
        {
            return new MarkerIcon()
                       {
                           Path = GetIconForCategory(categoryId),
                           Anchor = new Point(10, 34),
                           Point = Point.Empty,
                           Size = new Size(20, 34)
                       };
        }

        public string GetPlaceLogoUrl(PlaceDocument place)
        {
            if (place.Logo.HasValue())
            {
                return Path.Combine(UserFiles, "Places", place.Id, place.Logo).Replace("\\", "/"); 
            }
            return DefaultPlaceLogo;
        }

        private string DefaultPlaceLogo
        {
            get { return Path.Combine(UserFiles, "Places","default-logo.png").Replace("\\", "/"); }
        }

        public MarkerIcon FriendShadowModel
        {
            get
            {
                return new MarkerIcon
                           {
                               Path = "/UserFiles/border.png",
                               Size = new Size(60, 60),
                               Point = new Point(0, 0),
                               Anchor = new Point(0, 60)
                           };
            }
        }

        public MarkerIcon FriendIconModel
        {
            get
            {
                return new MarkerIcon
                {
                    Size = new Size(50, 50),
                    Point = new Point(0, 0),
                    Anchor = new Point(-7, 57)
                };
            }
        }

        public MarkerIcon EventShadowModel
        {
            get { return null; }
        }
    }
}