using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Events;
using mPower.Framework;

namespace YouMap.Domain
{
    public class PlaceAR : YoumapAR
    {
        public PlaceAR()
        {
            
        }

        public PlaceAR(PlaceData data, ICommandMetadata metadata)
        {
            SetCommandMetadata(metadata);
            _id = data.Id;
            Apply(new Place_AddedEvent
            {
                Id = data.Id,
                Title = data.Title,
                Description = data.Description,
                Address = data.Address,
                CreatorId = data.CreatorId,
                Icon = data.Icon,
                Latitude = data.Latitude,
                Longitude = data.Longitude
            });
        }

        public void UpdateLocation(string placeId, Location location)
        {
            Apply(new Place_LocationChanged
                      {
                          Id = placeId,
                          Latitude = location.Latitude,
                          Longitude = location.Longitude
                      });
        }

        #region Object Reconstruction

        protected void On(Place_AddedEvent place)
        {
            _id = place.Id;
        }

        #endregion
    }
}