namespace YouMap.Framework.Helpers
{
    public static class SelectListHelper
    {
        public static IEnumerable<SelectListItem> DaysOfWeek()
        {
            return new List<SelectListItem>
                       {
                           CreateSelectListItem(DayOfWeek.Monday,"Пн"),
                           CreateSelectListItem(DayOfWeek.Tuesday,"Вт"),
                           CreateSelectListItem(DayOfWeek.Wednesday,"Ср"),
                           CreateSelectListItem(DayOfWeek.Thursday,"Чт"),
                           CreateSelectListItem(DayOfWeek.Friday,"Пт"),
                           CreateSelectListItem(DayOfWeek.Saturday,"Сб"),
                           CreateSelectListItem(DayOfWeek.Sunday,"Вс"),
                       };
        }

         public static IEnumerable<SelectListItem> WorkDaysOfWeek()
         {
             return new List<SelectListItem>
                        {
                            CreateSelectListItem(DayOfWeek.Monday, "Пн", true),
                            CreateSelectListItem(DayOfWeek.Tuesday, "Вт", true),
                            CreateSelectListItem(DayOfWeek.Wednesday, "Ср", true),
                            CreateSelectListItem(DayOfWeek.Thursday, "Чт", true),
                            CreateSelectListItem(DayOfWeek.Friday, "Пт", true),
                            CreateSelectListItem(DayOfWeek.Saturday, "Сб"),
                            CreateSelectListItem(DayOfWeek.Sunday, "Вс")
                        };
         }

        public static SelectListItem CreateSelectListItem(object value, string text, bool selected = false)
        {
            return new SelectListItem {Text = text, Value = value.ToString(), Selected = selected};
        }
    }
}