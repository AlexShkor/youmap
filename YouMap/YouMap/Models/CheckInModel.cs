﻿using YouMap.Controllers;
using YouMap.Domain.Data;

namespace YouMap.Models
{
    public class CheckInModel
    {
        public PlaceModel PlaceModel { get; set; }

        public bool DisplayPlace { get; set; }

        public string Memo { get; set; }

        public string Title { get; set; }

        public Location Location { get; set; }

        public string PlaceId { get; set; }
    }
}