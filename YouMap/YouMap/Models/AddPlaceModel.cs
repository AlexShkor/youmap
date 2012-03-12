using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using YouMap.Controllers;

namespace YouMap.Models
{
    public class AddPlaceModel
    {
        [Required(ErrorMessage = "Введите название")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Введите адресс")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Адресс не найден, укажите адресс и нажмите кнопку поиска. Перетащите маркер на нуэную позицию, если требуется.")]
        public string Latitude { get; set; }

        [Required(ErrorMessage = "Ошибка.")]
        public string Longitude { get; set; }

        public IEnumerable<DayOfWeek> WorkDays { get; set; }

        public IEnumerable<SelectListItem> DaysOfWeek { get; set; } 
        
        [Required(ErrorMessage = "Выберите категорию")]
        public string CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public string Description { get; set; }

        public bool DisplayMap { get; set; }

        public MapModel Map { get; set; }

        public AddPlaceModel()
        {
            DaysOfWeek = YouMap.Framework.Helpers.SelectListHelper.WorkDaysOfWeek();
        }
    }
}