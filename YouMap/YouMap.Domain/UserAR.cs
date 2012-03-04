using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Events;
using mPower.Framework;

namespace YouMap.Domain
{
    public class UserAR : YoumapAR
    {

        public UserAR()
        {
            
        }

        public UserAR(string id, string email, string password, ICommandMetadata metadata)
        {
            _id = id;
            SetCommandMetadata(metadata);
            Apply(new User_CreatedEvent
                      {
                          UserId = id,
                          Email = email,
                          Password = password
                      });
        }

        public UserAR(string userId, VkData vkData, ICommandMetadata metadata)
        {
            _id = userId;
            SetCommandMetadata(metadata);
            Apply(new User_CreatedEvent
                      {
                          UserId = userId,
                          Vk = vkData
                      });
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

        #region Object Reconstruction

        protected void On(User_CreatedEvent created)
        {
            _id = created.UserId;
        }

        protected void On(User_ImportedFromVkEvent user)
        {
            _id = user.UserId;
        }

        #endregion
    }
}