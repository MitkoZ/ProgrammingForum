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
        public ViewQuestionViewModel(Question question)
        {
            this.QuestionId = question.Id;
            this.Title = question.Title;
            this.DateAsked = question.DateAsked;
        }

        public ViewQuestionViewModel()
        {
        }
    }
}