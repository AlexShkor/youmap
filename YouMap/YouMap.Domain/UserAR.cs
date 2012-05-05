using System;
using System.Collections.Generic;
using System.Linq;
using Paralect.Domain;
using YouMap.Domain.Data;
using YouMap.Domain.Events;
using YouMap.Framework;

namespace YouMap.Domain
{
    public class UserAR : YoumapAR
    {
        public HashSet<string> Friends { get; set; }  

        public UserAR()
        {
            Friends  = new HashSet<string>();
            Feeds = new HashSet<string>();
        }

        public UserAR(string userId, UserData userData, ICommandMetadata metadata): this()
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
                          Members = data.Members.ToList()
                      });
        }

        public void AddMemberToEvent(string newMemberId, string newMemberName, string eventId)
        {
            Apply(new User_EventMemberAddedEvent
                      {
                          UserId = _id,
                          NewMemberId = newMemberId,
                          NewMemberName = newMemberName,
                          EventId = eventId
                      });
        }

        public void AddFriends(List<Friend> friends)
        {
            var newFriends = friends.Where(x => !Friends.Contains(x.VkId)).ToList();
            if (newFriends.Any())
            {
                Apply(new User_FriendsAddedEvent
                          {
                              UserId = _id,
                              Friends = newFriends
                          });
            }
        }

        public void CreateFeed(string name)
        {
            Apply(new User_FeedCreatedEvent
            {
                UserId = Id,
                Name = name
            });
        }

        public HashSet<string> Feeds { get; set; }

        public void SubscribeFeed(string feed)
        {
            if (!Feeds.Contains(feed))
            {
                Apply(new User_FeedSubscribedEvent
                          {
                              UserId = Id,
                              Feed = feed
                          });
            }
        }


        public void UnsubscribeFeed(string feed)
        {
            if (Feeds.Contains(feed))
            {
                Apply(new User_FeedUnsubscribedEvent
                          {
                              UserId = Id,
                              Feed = feed
                          });
            }
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

        protected void On(User_FeedSubscribedEvent user)
        {
            Feeds.Add(user.Feed);
        }

        protected void On(User_FeedUnsubscribedEvent user)
        {
            Feeds.Remove(user.Feed);
        }

        protected void On(User_ImportedFromVkEvent user)
        {
            _id = user.UserId;
        }

        protected void On(User_FriendsAddedEvent user)
        {
            Friends.SymmetricExceptWith(user.Friends.Select(x=> x.VkId));
        }
        #endregion
    }
}