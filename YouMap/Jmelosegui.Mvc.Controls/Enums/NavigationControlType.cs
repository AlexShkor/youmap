using Telerik.Web.Mvc.UI;

namespace Jmelosegui.Mvc.Controls
{
    public enum NavigationControlType
    {
        [ClientSideEnumValue("'DEFAULT'")]
        Default,
        [ClientSideEnumValue("'ANDROID'")]
        Android,
        [ClientSideEnumValue("'SMALL'")]
        Small,
        [ClientSideEnumValue("'ZOOM_PAN'")]
        ZoomPan

    }
}
