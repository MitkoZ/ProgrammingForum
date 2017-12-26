using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DataAccess.Models;

namespace ProgrammingForum.ViewModels
{
    public class ViewQuestionViewModel
    {
        public int QuestionId { get; set; }
        [Required]
        public string Title { get; set; }
        [Display(Name = "Date Asked")]
        public DateTime DateAsked { get; set; }
        [Display(Name = "Question")]
        [Required]
        public string QuestionText { get; set; }
        public List<Comment> Comments { get; set; }
        public string CommentText { get; set; }
        public string AskedBy { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Categories")]
        public List<CategoryViewModel> ChosenCategories { get; set; }
        public ViewQuestionViewModel(Question question)
        {
            this.QuestionId = question.Id;
            this.Title = question.Title;
            this.QuestionText = question.QuestionText;
            this.DateAsked = question.DateAsked;
            this.UserId = question.UserId;
            this.ChosenCategories = new List<CategoryViewModel>();
            question.Categories.ToList().ForEach(x => ChosenCategories.Add(new CategoryViewModel(x)));//gets the categories of the question
        }

        public ViewQuestionViewModel()
        {
        }
    }
}