using System;

namespace YouMap.Domain.Data
{
    //used in events
    public class VkData
    {
        public long Expire { get; set; }

        public string Sig { get; set; }

        public string Sid { get; set; }

        public string Mid { get; set; }

        public string Secret { get; set; }

        public String Domain { get; set; }

        public String FirstName { get; set; }

        public string LastName { get; set; }

        //TODO: change to long
        public string Id { get; set; }

        public String Href { get; set; }

        public String Nickname { get; set; }
    }
}