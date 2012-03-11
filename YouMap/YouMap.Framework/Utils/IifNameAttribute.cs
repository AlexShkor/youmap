using System.ComponentModel;

namespace YouMap.Framework.Utils
{
    public class IifNameAttribute : DescriptionAttribute
    {
        public IifNameAttribute()
        {
        }

        public IifNameAttribute(string description)
            : base(description)
        {
        }
    }
}
