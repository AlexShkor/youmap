using System;
using System.Collections.Generic;
using System.Linq;
using YouMap.Framework.Extensions;
using YouMap.Models;

namespace YouMap.Helpers
{
    public class RandomHelper
    {
        static Random random = new Random();


        public static IEnumerable<FriendMarkerModel> GenerateFriendMarkers(params string[] ids)
        {
            var result = ids.Select(x => new FriendMarkerModel
                                             {
                                                 Id = x,
                                                 X = RandomMinskLatitude(),
                                                 Y = RandomMinskLongitude(),
                                                 Visited = PrettyDate(),
                                             });
            return result;
        }

        public static double RandomMinskLatitude()
        {
            return 53.90234 + (random.NextDouble() - random.NextDouble())/10;
        }

        public static double RandomMinskLongitude()
        {
            return 27.561896 + (random.NextDouble() - random.NextDouble()) / 10;
        }

        public static string PrettyDate()
        {
            return  (DateTime.Now - DateTime.Now.AddDays(-random.Next(100))).ToDisplayString();
        }
    }
}