namespace YouMap.Models
{
    public class VkPanelModel
    {
        public bool ShowFriends { get; set; }
        public bool ShareLocation { get; set; }
        public bool IsVkUser { get; set; }

        public VkPanelModel()
        {
            ShareLocation = true;
            ShareLocation = true;
        }
    }
}