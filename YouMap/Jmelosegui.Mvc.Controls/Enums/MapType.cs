using Telerik.Web.Mvc.UI;

namespace Jmelosegui.Mvc.Controls
{
    public enum MapType
    {
        [ClientSideEnumValue("'HYBRID'")]
        Hybrid,
        [ClientSideEnumValue("'ROADMAP'")]
        Roadmap,
        [ClientSideEnumValue("'SATELLITE'")]
        Satellite,
        [ClientSideEnumValue("'TERRAIN'")]
        Terrain
    }
}
