using System;
using Paralect.Domain;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Domain.Events;
using YouMap.Framework;

namespace YouMap.Domain
{
    public class UserAR : YoumapAR
    {
        public UserAR()
        {
            
        }

        public UserAR(string userId, UserData userData, ICommandMetadata metadata)
        {
            _id = userId;
            SetCommandMetadata(metadata);
            Apply(new User_CreatedEvent
            {
                UserId = userId,
                Vk = userData.Vk,
                Password = userData.Password,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                UserName = userData.UserName,
                Email = userData.Email,
                Permissions = userData.Permissions
            });
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            Apply(new User_PasswordChangedEvent
                      {
                          UserId = _id,
                          NewPassword = newPassword,
                          OldPassword = oldPassword
                      }
                );
        }

        public void ImportFromVk(VkData vk)
        {
            Apply(new User_ImportedFromVkEvent
            {
                UserId = _id,
                Vk = vk
            });
        }

        public void UpdateMark(Location location)
        {
            Apply(new User_MarkUdatedEvent
            {
                Location = location,
                UserId = _id,
                Visited = DateTime.Now
            });
        }

        public void AddCheckIn(CheckInData data)
        {
            Apply(new User_CheckInAddedEvent
                      {
                          Location = data.Location,
                          Memo = data.Memo,
                          PlaceId = data.PlaceId,
                          Title = data.Title,
                          UserId = _id,
                          Visited = DateTime.Now
                      });
        }

        #region Object Reconstruction

        protected void On(User_CreatedEvent created)
        {
            _id = created.UserId;
        }

        protected void On(User_MarkUdatedEvent user)
        {
        }

        protected void On(User_PasswordChangedEvent user)
        {
        }

        protected void On(User_ImportedFromVkEvent user)
        {
            _id = user.UserId;
        }

        #endregion

        public void AddEvent(IEventData data)
        {
            Apply(new User_EventAddedEvent
                      {
                          UserId = _id,
                          Id = data.EventId,
                          Location = data.Location,
                          Memo = data.Memo,
                          OwnerId = data.OwnerId,
                          Title = data.Title,
                          Private = data.Private,
                          PlaceId = data.PlaceId,
                          Start = data.Start,
                          UsersIds = data.UsersIds
                      });
        }

        public void AddMemberToEvent(string newMemberId, string eventId)
        {
            Apply(new User_EventMemberAddedEvent
                      {
                          UserId = _id,
                          NewMemberId = newMemberId,
                          EventId = eventId
                      });
        }
    }
}