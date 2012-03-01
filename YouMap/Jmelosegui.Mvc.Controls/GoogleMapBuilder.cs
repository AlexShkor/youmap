using System;
using System.Collections.Generic;
using Jmelosegui.Mvc.Controls.Overlays;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Infrastructure;
using Telerik.Web.Mvc.UI;
using Jmelosegui.Mvc.Controls.Overlays.Markers;

namespace Jmelosegui.Mvc.Controls
{
    public class GoogleMapBuilder : ViewComponentBuilderBase<GoogleMap, GoogleMapBuilder>, IHideObjectMembers
    {
        public GoogleMapBuilder(GoogleMap component) : base(component)
        {
        }

        #region Public Methods

        public IHtmlNode Build()
        {
            var content = new HtmlElement("div")
                .AddClass("t-googlemap")
                .Attribute("id", Component.Id)
                .Attributes(Component.HtmlAttributes);

            if (Component.Width != 0)
            {
                content.Css("width", Component.Width + "px");
            }

            if (Component.Height != 0)
            {
                content.Css("height", Component.Height + "px");
            }

            return content;
        }

        public GoogleMapBuilder ClientEvents(Action<GoogleMapClientEventsBuilder> clientEventsAction)
        {
            Guard.IsNotNull(clientEventsAction, "clientEventsAction");
            clientEventsAction(new GoogleMapClientEventsBuilder(Component.ClientEvents));
            return this;
        }

        public GoogleMapBuilder Circles(Action<CircleFactory> action)
        {
            Guard.IsNotNull(action, "action");
            var factory = new CircleFactory(Component);
            action(factory);
            return this;
        }

        public GoogleMapBuilder DisableDoubleClickZoom(bool disabled)
        {
            Component.DisableDoubleClickZoom = disabled;
            return this;
        }

        public GoogleMapBuilder Draggable(bool enabled)
        {
            Component.Draggable = enabled;
            return this;
        }

        public GoogleMapBuilder EnableMarkersClustering()
        {
            return EnableMarkersClustering(null);
        }

        public GoogleMapBuilder EnableMarkersClustering(Action<MarkerClusteringOptionsFactory> action)
        {
            Component.EnableMarkersClustering = true;
            if(action != null){
                var options = new MarkerClusteringOptionsFactory(Component);
                action(options);
            }
            return this;
        }

        public GoogleMapBuilder Height(int value)
        {
            Component.Height = value;
            return this;
        }

        public GoogleMapBuilder Latitude(double value)
        {
            Component.Latitude = value;
            return this;
        }

        public GoogleMapBuilder Longitude(double value)
        {
            Component.Longitude = value;
            return this;
        }

        public GoogleMapBuilder MapType(MapType value)
        {
            Component.MapType = value;
            return this;
        }

        public GoogleMapBuilder MapTypeControlPosition(ControlPosition controlPosition)
        {
            Component.MapTypeControlPosition = controlPosition;
            return this;
        }

        public GoogleMapBuilder MapTypeControlVisible(bool visible)
        {
            Component.MapTypeControlVisible = visible;
            return this;
        }

        public GoogleMapBuilder MapTypeControlStyle(MapTypeControlStyle value)
        {
            Component.MapTypeControlStyle = value;
            return this;
        }

        public GoogleMapBuilder Markers(Action<MarkerFactory> action)
        {
            Guard.IsNotNull(action, "action");
            var factory = new MarkerFactory(Component);
            action(factory);
            return this;
        }

        public GoogleMapBuilder NavigationControlType(NavigationControlType controlType)
        {
            Component.NavigationControlType = controlType;
            return this;
        }

        public GoogleMapBuilder NavigationControlPosition(ControlPosition controlPosition)
        {
            Component.NavigationControlPosition = controlPosition;
            return this;
        }

        public GoogleMapBuilder NavigationControlVisible(bool visible)
        {
            Component.NavigationControlVisible = visible;
            return this;
        }

        public GoogleMapBuilder Polygons(Action<PolygonFactory> action)
        {
            Guard.IsNotNull(action, "action");
            var factory = new PolygonFactory(Component);
            action(factory);
            return this;
        }

        public GoogleMapBuilder ScaleControlPosition(ControlPosition controlPosition)
        {
            Component.ScaleControlPosition = controlPosition;
            return this;
        }

        public GoogleMapBuilder ScaleControlVisible(bool visible)
        {
            Component.ScaleControlVisible = visible;
            return this;
        }

        public GoogleMapBuilder Width(int value)
        {
            Component.Width = value;
            return this;
        }

        public GoogleMapBuilder Zoom(int value)
        {
            Component.Zoom = value;
            return this;
        }

        #endregion

        [Obsolete("Use BindTo<T, TOverlay> instead")]
        public GoogleMapBuilder BindMarkersTo<T>(IEnumerable<T> dataSource, Action<OverlayBindingFactory<Marker>> itemDataBound)
        {
            return BindTo(dataSource, itemDataBound);
        }

        public GoogleMapBuilder BindTo<T, TOverlay>(IEnumerable<T> dataSource, Action<OverlayBindingFactory<TOverlay>> itemDataBound) where TOverlay : Overlay
        {
            Component.BindTo(dataSource, itemDataBound);
            return this;
        }

    }
}