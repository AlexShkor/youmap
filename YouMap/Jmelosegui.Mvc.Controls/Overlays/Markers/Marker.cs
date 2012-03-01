﻿using Telerik.Web.Mvc;

namespace Jmelosegui.Mvc.Controls.Overlays
{
    public class Marker : Overlay, IHideObjectMembers
    {
        private readonly int index = -1;
        public Marker(GoogleMap map) : base(map)
        {
            Clickable = true;
            Draggable = false;
            index = map.Markers.Count - 1;
        }

        public int Index
        {
            get { return index; }
        }

        public bool Clickable { get; set; }

        public bool Draggable { get; set; }

        public MarkerImage Icon { get; set; }

        public MarkerImage Shadow { get; set; }

        public string Title { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "z")]
        public int zIndex { get; set; }

        public InfoWindow Window { get; set; }

        public virtual IOverlaySerializer CreateSerializer()
        {
            return new MarkerSerializer(this);
        }
    }
}
