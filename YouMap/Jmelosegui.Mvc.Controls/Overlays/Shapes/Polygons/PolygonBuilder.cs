using Telerik.Web.Mvc.Infrastructure;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public class PolygonBuilder : ShapeBuilder<Polygon>
    {
        private readonly Polygon polygon;

        public PolygonBuilder(Polygon polygon): base(polygon)
        {
            this.polygon = polygon;
        }

        public virtual ShapeBuilder<Polygon> Points(Action<LocationFactory<Polygon>> action)
        {
            Guard.IsNotNull(action, "action");
            var factory = new LocationFactory<Polygon>(polygon);
            action(factory);
            return this;
        }
    }
}
