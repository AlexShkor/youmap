using System.Linq;
using System.Text;
using Telerik.Web.Mvc;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public class PolygonFactory : IHideObjectMembers
    {
        private readonly GoogleMap map;

        public PolygonFactory(GoogleMap map)
        {
            this.map = map;
        }

        public PolygonBuilder Add()
        {
            var polygon = new Polygon(map);

            map.Polygons.Add(polygon);

            return new PolygonBuilder(polygon);
        }
    }
}
