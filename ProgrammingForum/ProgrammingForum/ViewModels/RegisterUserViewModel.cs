using ProgrammingForum.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProgrammingForum.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [CustomPassword]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Repeat Password")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [CustomPassword]
        public string RepeatPassword { get; set; }
    }
}