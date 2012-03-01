using System.Collections.Generic;
using Telerik.Web.Mvc.Infrastructure;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public class CircleSerializer : ShapeSerializer<Circle>
    {
        private readonly Circle circle;

        public CircleSerializer(Circle circle) : base(circle)
        {
            this.circle = circle;
        }

        public override IDictionary<string, object> Serialize()
        {
            IDictionary<string, object> result = base.Serialize();
            FluentDictionary.For(result)
                .Add("Center", circle.Center)
                .Add("Radius", circle.Radius);

            return result;
        }
    }
}