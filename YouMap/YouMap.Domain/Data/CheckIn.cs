using System;

namespace YouMap.Domain.Data
{
    public class CheckIn
    {
        public Location Location { get; set; }

        public DateTime Visited { get; set; }

        public string Memo { get; set; }

        public string Title { get; set; }

        public bool IsHidden { get; set; }
    }
}