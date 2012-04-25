namespace YouMap.Models
{
    public class UserInfoModel
    {
        public string Id { get; set; }

        public int CheckInsCount { get; set; }

        public int EventsCount { get; set; }

        public int FriendsCount { get; set; }

        public string LastCheckInTimeAgo { get; set; }

        public string LastCheckInMessage { get; set; }

        public string PlaceTitle { get; set; }

        public string PlaceId { get; set; }

        public string VkLink
        {
            get { return "http://vk.com/id" + VkId; }
        }

        protected string VkId { get; set; }

        public string Name { get; set; }
    }
}