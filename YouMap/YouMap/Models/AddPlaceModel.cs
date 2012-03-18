using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using YouMap.Controllers;

namespace YouMap.Models
{
    public class AddPlaceModel
    {
        [Required(ErrorMessage = "������� ��������")]
        public string Title { get; set; }

        [Required(ErrorMessage = "������� ������")]
        public string Address { get; set; }

        [Required(ErrorMessage = "������ �� ������, ������� ������ � ������� ������ ������. ���������� ������ �� ������ �������, ���� ���������.")]
        public string Latitude { get; set; }

        [Required(ErrorMessage = "������.")]
        public string Longitude { get; set; }

        public IEnumerable<DayOfWeek> WorkDays { get; set; }

        public IEnumerable<SelectListItem> DaysOfWeek { get; set; } 
        
        [Required(ErrorMessage = "�������� ���������")]
        public string CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public string Description { get; set; }

        public bool DisplayMap { get; set; }

        public MapModel Map { get; set; }

        public string LogoFileName { get; set; }

        public string Id { get; set; }

        public HttpPostedFileBase LogoFile { get; set; }

        public AddPlaceModel()
        {
            DaysOfWeek = YouMap.Framework.Helpers.SelectListHelper.WorkDaysOfWeek();
        }
    }
}