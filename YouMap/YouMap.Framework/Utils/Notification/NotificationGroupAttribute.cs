using System;

namespace YouMap.Framework.Utils.Notification
{
    public class NotificationGroupAttribute : Attribute
    {
        public NotificationGroupEnum Group { get; private set; }

        public NotificationGroupAttribute(NotificationGroupEnum group)
        {
            Group = group;
        }
    }
}