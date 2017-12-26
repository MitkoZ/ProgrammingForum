using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProgrammingForum.ViewModels
{
    public class UserCommentViewModel
    {
        public string Lineage { get; set; }
        public string CommentedTextFormatted { get; set; }
        public string DateCommented { get; set; }
        public string Username { get; set; }
        public string ParentCommentId { get; set; }
        public int CommentId { get; set; }
        public string CommentTo { get; set; }
        public int UserId { get; set; }
    }
}