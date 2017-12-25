using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProgrammingForum.ViewModels
{
    public class UserCommentsAndQuestionViewModel
    {
        public List<UserCommentViewModel> UsersCommentsViewModel { get; set; }
        public ViewQuestionViewModel ViewQuestionViewModel { get; set; }
    }
}