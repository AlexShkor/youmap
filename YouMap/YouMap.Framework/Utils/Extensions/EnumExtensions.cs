using System;
using System.ComponentModel;
using System.Linq;
using mPower.Framework.Utils.Notification;

namespace mPower.Framework.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetNullableDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? null : attribute.Description;
        }

        public static string GetIifName(this Enum value)
        {
            var attribute = value.GetAttribute<IifNameAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static NotificationGroupEnum GetNotificationGroup(this Enum value)
        {
            var attribute = value.GetAttribute<NotificationGroupAttribute>();
            return attribute == null ? NotificationGroupEnum.System : attribute.Group;
        }

        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return null;
            var attributes = (T[]) fi.GetCustomAttributes(typeof (T), false);

            return attributes.SingleOrDefault(x => x.GetType() == typeof (T));
        }
    }
}
