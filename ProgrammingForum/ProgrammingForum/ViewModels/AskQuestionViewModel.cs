using DataAccess.Models;
using ProgrammingForum.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProgrammingForum.ViewModels
{
    public class AskQuestionViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Question")]
        public string QuestionText { get; set; }
        [CustomChosenCategoriesAttribute]
        public List<int> ChosenCategoriesIds { get; set; }
        public List<CategoryViewModel> AllCategories { get; set; }
        public AskQuestionViewModel()
        {
            AllCategories = new List<CategoryViewModel>();
        }
    }
}