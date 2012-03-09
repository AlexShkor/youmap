using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
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
                CategoryId = data.CategoryId,
                WorkDays = data.WorkDays,
                Location = data.Location
            });
        }

        public void UpdateLocation(string placeId, Location location)
        {
            Apply(new Place_LocationChanged
                      {
                          Id = placeId,
                          Location = location
                      });
        }

        #region Object Reconstruction

        protected void On(Place_AddedEvent place)
        {
            _id = place.Id;
        }

        #endregion

        public void ChangeStatus(PlaceStatusEnum status)
        {
            Apply(new Place_StatusChangedEvent
                      {
                          PlaceId = _id,
                          Status = status
                      });
        }
    }
}