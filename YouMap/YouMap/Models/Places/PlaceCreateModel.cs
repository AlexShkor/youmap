using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Controllers;

namespace YouMap.Models
{
    public class PlaceCreateModel
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

        [Range(0,5)]
        public int Layer { get; set; }

        public IEnumerable<SelectListItem> Layers { get; set; } 

        public string Id { get; set; }

        public HttpPostedFileBase LogoFile { get; set; }

        public List<string> Tags { get; set; }

        public PlaceCreateModel()
        {
            DaysOfWeek = Framework.Helpers.SelectListHelper.WorkDaysOfWeek();
            Layers = new SelectList(Enumerable.Range(0,6));
            Tags = new List<string>();
        }
    }
}