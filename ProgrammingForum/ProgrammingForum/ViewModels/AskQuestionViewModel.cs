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
        public List<CategoryViewModel> CheckedCategories { get; set; }

        public int QuestionId { get; set; }
        public AskQuestionViewModel()
        {
            this.AllCategories = new List<CategoryViewModel>();
            this.CheckedCategories = new List<CategoryViewModel>();
            this.ChosenCategoriesIds = new List<int>();
        }

        public AskQuestionViewModel(List<Category> chosenCategories) : this()
        {
            foreach (Category category in chosenCategories)
            {
                CheckedCategories.Add(new CategoryViewModel(category));
            }
        }
    }
}