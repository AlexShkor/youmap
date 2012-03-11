using Telerik.Web.Mvc.Infrastructure;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public class ShapeSerializer<TShape>: IOverlaySerializer where TShape : Shape
    {
        private readonly TShape shape;

        public ShapeSerializer(TShape shape)
        {
            this.shape = shape;
        }

        public virtual IDictionary<string, object> Serialize()
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            FluentDictionary.For(result)
                .Add("Clickable", shape.Clickable, () => shape.Clickable)
                .Add("FillColor", shape.FillColor.ToHtml())
                .Add("FillOpacity", shape.FillOpacity)
                .Add("StrokeColor", shape.StrokeColor.ToHtml())
                .Add("StrokeOpacity", shape.StrokeOpacity)
                .Add("StrokeWeight", shape.StrokeWeight);

            return result;
        }
    }
}
