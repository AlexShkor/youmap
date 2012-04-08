using System.Web.Mvc;

namespace YouMap.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Mobile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Mobile_default",
                "Mobile/{controller}/{action}/{id}",
                new {action = "Main", controller = "Home", id = UrlParameter.Optional }
            , new string[] { "YouMap.Areas.Mobile.Controllers" });
        }
    }
}
