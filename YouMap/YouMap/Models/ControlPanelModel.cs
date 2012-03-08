namespace YouMap.Models
{
    public class ControlPanelModel
    {
        public bool Visible { get { return IsAdminPanelVisible || IsAdvertiserPanelVisible; }}

        public bool IsAdminPanelVisible { get; set; }

        public bool IsAdvertiserPanelVisible { get; set; }
    }
}