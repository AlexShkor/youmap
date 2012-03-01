﻿namespace Jmelosegui.Mvc.Controls
{
    public abstract class Overlay
    {
        private readonly GoogleMap map;
        private double longitude;
        private double latitude;

        protected Overlay(GoogleMap map)
        {
            this.map = map;
        }

        protected internal GoogleMap Map
        {
            get { return this.map; }
        }
        public virtual double Longitude
        {
            get
            {
                if (longitude == 0)
                    longitude = map.Longitude;
                return longitude;
            }
            set { longitude = value; }
        }

        public virtual double Latitude
        {
            get
            {
                if (latitude == 0)
                    latitude = map.Latitude;
                return latitude;
            }
            set { latitude = value; }
        }
    }
}