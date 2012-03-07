using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace YouMap.Models
{

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "{0} - обязательное поле.")]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "{0} - обязательное поле.")]
        [StringLength(100, ErrorMessage = "{0} должен быть минимум из {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Подтверждения новго пароля не верно.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required(ErrorMessage = "{0} - обязательное поле.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} - обязательное поле.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "{0} - обязательное поле.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} - обязательное поле.")]
        [StringLength(100, ErrorMessage = "{0} должен быть минимум из {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} - обязательное поле.")]
        [Display(Name = "Подтвердите пароля")]
        [Compare("Password", ErrorMessage = "Подтверждения новго пароля не верно.")]
        public string ConfirmPassword { get; set; }
    }
}
