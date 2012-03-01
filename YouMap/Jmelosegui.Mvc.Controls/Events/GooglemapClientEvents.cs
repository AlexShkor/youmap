using Telerik.Web.Mvc.UI;

namespace Jmelosegui.Mvc.Controls
{
    public class GoogleMapClientEvents
    {
        public GoogleMapClientEvents()
        {
            OnLoad = new ClientEvent();
            OnClick = new ClientEvent();
        }

        public ClientEvent OnLoad { get; private set; }

        public ClientEvent OnClick { get; private set; }
    }

}
