using System;
using YouMap.Domain.Data;

namespace YouMap.Documents.Documents
{
    public class CheckInDocument
    {
        public Location Location { get; set; }

        public DateTime Visited { get; set; }

        public string Memo { get; set; }

        public string Title { get; set; }

        public bool IsHidden { get; set; }

        public string PlaceId { get; set; }
    }

}