using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Jmelosegui.Mvc.Controls.Overlays;
using Telerik.Web.Mvc.Extensions;
using Telerik.Web.Mvc.Infrastructure;
using Telerik.Web.Mvc.UI;

namespace Jmelosegui.Mvc.Controls
{
    public class GoogleMap : ViewComponentBase
    {
        #region Public Properties

        public GoogleMapClientEvents ClientEvents { get; private set; }

        public bool DisableDoubleClickZoom { get; set; }

        public bool Draggable { get; set; }

        public bool EnableMarkersClustering { get; set; }

        public int Height { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public MapType MapType { get; set; }

        public MapTypeControlStyle MapTypeControlStyle { get; set; }

        public ControlPosition MapTypeControlPosition { get; set; }

        public bool MapTypeControlVisible { get; set; }

        public IList<Marker> Markers { get; private set; }

        public MarkerClusteringOptions MarkerClusteringOptions { get; private set; }

        public NavigationControlType NavigationControlType { get; set; }

        public ControlPosition NavigationControlPosition { get; set; }

        public bool NavigationControlVisible { get; set; }

        public IList<Polygon> Polygons { get; private set; }

        public IList<Circle> Circles { get; private set; }

        public bool ScaleControlVisible { get; set; }

        public ControlPosition ScaleControlPosition { get; set; }

        public int Width { get; set; }

        public int Zoom { get; set; }

        #endregion

        #region Constructor

        public GoogleMap(ViewContext viewContext, IClientSideObjectWriterFactory clientSideObjectWriterFactory)
            : base(viewContext, clientSideObjectWriterFactory)
        {
            ScriptFileNames.AddRange(new[] { "telerik.common.js", "jmelosegui.googlemap.js" });

            Initialize();
        }

        private void Initialize()
        {
            ClientEvents = new GoogleMapClientEvents();
            DisableDoubleClickZoom = false;
            Draggable = true;
            EnableMarkersClustering = false;
            Latitude = 23;
            Longitude = -82;
            MapType = MapType.Roadmap;
            MapTypeControlPosition = ControlPosition.TopRight;
            MapTypeControlVisible = true;
            Markers = new List<Marker>();
            MarkerClusteringOptions = new MarkerClusteringOptions();
            Polygons = new List<Polygon>();
            Circles = new List<Circle>();
            NavigationControlType = NavigationControlType.Default;
            NavigationControlPosition = ControlPosition.TopLeft;
            NavigationControlVisible = true;
            ScaleControlVisible = false;
            ScaleControlPosition = ControlPosition.BottomLeft;
            Height = 300;
            Width = 550;            
        }

        #endregion

        #region Override Methods

        public override void WriteInitializationScript(System.IO.TextWriter writer)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            IClientSideObjectWriter objectWriter = ClientSideObjectWriterFactory.Create(Id, "GoogleMap", writer);

            objectWriter.Start();

            objectWriter.Append("ClientID", Id);
            objectWriter.Append("DisableDoubleClickZoom", DisableDoubleClickZoom, false);
            objectWriter.Append("Draggable", Draggable, true);
            objectWriter.Append("EnableMarkersClustering", EnableMarkersClustering, false);
            objectWriter.Append("Height", Height);
            objectWriter.Append("Latitude", Latitude);
            objectWriter.Append("Longitude", Longitude);
            objectWriter.Append("MapType", MapType, MapType.Roadmap);
            objectWriter.Append("MapTypeControlPosition", MapTypeControlPosition, ControlPosition.TopRight);
            objectWriter.Append("MapTypeControlVisible", MapTypeControlVisible, true);
            objectWriter.Append("MapTypeControlStyle", MapTypeControlStyle, MapTypeControlStyle.Default);
            objectWriter.Append("NavigationControlPosition", NavigationControlPosition, ControlPosition.TopRight);
            objectWriter.Append("NavigationControlType", NavigationControlType, NavigationControlType.Default);
            objectWriter.Append("NavigationControlVisible", NavigationControlVisible, true);
            objectWriter.Append("ScaleControlVisible", ScaleControlVisible, false);
            objectWriter.Append("ScaleControlPosition", ScaleControlPosition, ControlPosition.BottomLeft);
            objectWriter.Append("Width", Width);
            objectWriter.Append("Zoom", (Zoom == 0) ? 6 : Zoom);

            if (EnableMarkersClustering)
            {
                objectWriter.AppendObject("MarkerClusteringOptions", MarkerClusteringOptions.Serialize());
            }

            if (Markers.Any())
            {
                var markers = new List<IDictionary<string, object>>();

                Markers.Each(m => markers.Add(m.CreateSerializer().Serialize()));

                if (markers.Any())
                {
                    objectWriter.AppendCollection("Markers", markers);
                }
            }

            if (Polygons.Any())
            {
                var polygons = new List<IDictionary<string, object>>();

                Polygons.Each(p => polygons.Add(p.CreateSerializer().Serialize()));

                if (polygons.Any())
                {
                    objectWriter.AppendCollection("Polygons", polygons);
                }
            }

            if (Circles.Any())
            {
                var circles = new List<IDictionary<string, object>>();

                Circles.Each(c => circles.Add(c.CreateSerializer().Serialize()));

                if (circles.Any())
                {
                    objectWriter.AppendCollection("Circles", circles);
                }
            }

            objectWriter.AppendClientEvent("onLoad", ClientEvents.OnLoad);
            objectWriter.AppendClientEvent("onClick", ClientEvents.OnClick);

            objectWriter.Complete();

            base.WriteInitializationScript(writer);

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        protected override void WriteHtml(System.Web.UI.HtmlTextWriter writer)
        {
            Guard.IsNotNull(writer, "writer");

            writer.Write(@"<script type=""text/javascript"" src=""http://maps.google.com/maps/api/js?sensor=false""></script>");
            if (EnableMarkersClustering)
                ScriptFileNames.Add("markerclusterer.js");
            var builder = new GoogleMapBuilder(this);
            IHtmlNode rootTag = builder.Build();
            rootTag.WriteTo(writer);

            if (Markers.Any(m => m.Window != null))
            {
                //Build Container for InfoWindows
                IHtmlNode infoWindowsRootTag = new HtmlElement("div")
                    .Attribute("id", "{0}-InfoWindowsHolder".FormatWith(Id))
                    .Attribute("style", "display: none");

                Markers.Where(m => m.Window != null).Each(m =>
                {
                    IHtmlNode markerInfoWindows = new HtmlElement("div")
                        .Attribute("id", "{0}Marker{1}".FormatWith(Id, m.Index));
                    m.Window.Template.Apply(markerInfoWindows);
                    infoWindowsRootTag.Children.Add(markerInfoWindows);
                });

                infoWindowsRootTag.WriteTo(writer);
            }
            base.WriteHtml(writer);
        }

        #endregion

        public virtual void BindTo<TGoogleMapOverlay, TDataItem>(IEnumerable<TDataItem> dataSource, Action<OverlayBindingFactory<TGoogleMapOverlay>> action)
            where TGoogleMapOverlay : Overlay
        {
            Guard.IsNotNull(action, "action");
            var factory = new OverlayBindingFactory<TGoogleMapOverlay>();
            action(factory);

            foreach (TDataItem dataItem in dataSource)
            {
                Overlay overlay = null;

                switch (typeof(TGoogleMapOverlay).FullName)
                {
                    case "Jmelosegui.Mvc.Controls.Overlays.Marker":
                        overlay = new Marker(this);
                        Markers.Add((Marker) overlay);
                        break;
                    case "Jmelosegui.Mvc.Controls.Overlays.Circle":
                        overlay = new Circle(this);
                        Circles.Add((Circle) overlay);
                        break;
                    case "Jmelosegui.Mvc.Controls.Overlays.Polygon":
                        overlay = new Polygon(this);
                        Polygons.Add((Polygon) overlay);
                        break;
                }

                factory.Binder.ItemDataBound((TGoogleMapOverlay)overlay, dataItem);
            }
        }
    }

}
