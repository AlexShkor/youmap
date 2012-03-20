using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;
using YouMap.Domain.Events;
using YouMap.Framework;

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
                Status = data.Status,
                CategoryId = data.CategoryId,
                Logo = data.Logo,
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

        public void AssignToUser(string userId)
        {
            Apply(new Place_AssignedEvent
                      {
                          PlaceId = _id,
                          OwnerId = userId
                      });
        }

        public void Update(PlaceData data)
        {
            Apply(new Place_UpdatedEvent
            {
                Id = _id,
                Title = data.Title,
                Description = data.Description,
                Address = data.Address,
                CreatorId = data.CreatorId,
                CategoryId = data.CategoryId,
                Logo = data.Logo,
                WorkDays = data.WorkDays,
                Location = data.Location
            });
        }
    }
}