using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProgrammingForum.ViewModels
{
    public class CommentACommentViewModel
    {
        public int QuestionId { get; set; }
        public int CommentId { get; set; }
        public string CommentText { get; set; }
    }
}