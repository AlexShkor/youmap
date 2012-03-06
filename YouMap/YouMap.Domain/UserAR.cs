using System;
using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Events;
using mPower.Framework;

namespace YouMap.Domain
{
    public class UserAR : YoumapAR
    {
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
        }

        #endregion

        
    }
}