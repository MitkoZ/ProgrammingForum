using DataAccess.Models;
using DataAccess.Repositories;
using ProgrammingForum.Helpers;
using ProgrammingForum.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProgrammingForum.Controllers
{
    public class QuestionsController : Controller
    {
        [CustomAuthorize(AllowAccessToUser = true)]
        [HttpGet]
        public ActionResult AskQuestion()
        {
            AskQuestionViewModel askQuestionViewModel = new AskQuestionViewModel();
            UnitOfWork unitOfWork = new UnitOfWork();
            unitOfWork.CategoryRepository.GetAll().ForEach(x => askQuestionViewModel.AllCategories.Add(new CategoryViewModel(x)));
            return View(askQuestionViewModel);
        }

        [CustomAuthorize(AllowAccessToUser = true)]
        [HttpPost]
        public ActionResult AskQuestion(AskQuestionViewModel askQuestionViewModel)
        {
            if (string.IsNullOrWhiteSpace(askQuestionViewModel.Title))
            {
                ModelState.AddModelError("", "Please enter a valid question title");
                UnitOfWork unitOfWork = new UnitOfWork();
                unitOfWork.CategoryRepository.GetAll().ForEach(x => askQuestionViewModel.AllCategories.Add(new CategoryViewModel(x)));
                return View(askQuestionViewModel);
            }

            if (string.IsNullOrWhiteSpace(askQuestionViewModel.QuestionText))
            {
                ModelState.AddModelError("", "Please enter a valid question");
                UnitOfWork unitOfWork = new UnitOfWork();
                unitOfWork.CategoryRepository.GetAll().ForEach(x => askQuestionViewModel.AllCategories.Add(new CategoryViewModel(x)));
                return View(askQuestionViewModel);
            }

            if (askQuestionViewModel.ChosenCategoriesIds.Count == 0)
            {
                ModelState.AddModelError("", "Please choose at least 1 category");
                UnitOfWork unitOfWork = new UnitOfWork();
                unitOfWork.CategoryRepository.GetAll().ForEach(x => askQuestionViewModel.AllCategories.Add(new CategoryViewModel(x)));
                return View(askQuestionViewModel);
            }

            if (ModelState.IsValid)
            {
                UnitOfWork unitOfWork = new UnitOfWork();
                Question questionInput = new Question();
                questionInput.Title = askQuestionViewModel.Title;
                questionInput.QuestionText = askQuestionViewModel.QuestionText;
                questionInput.DateAsked = DateTime.Now;
                questionInput.UserId = LoginUserSession.Current.UserID;
                questionInput.IsDeleted = false;
                askQuestionViewModel.ChosenCategoriesIds.ForEach(c => questionInput.Categories.Add(unitOfWork.CategoryRepository.GetFirst(x => x.Id == c)));
                unitOfWork.QuestionRepository.Create(questionInput);
                bool isAdded = unitOfWork.Save() > 0;
                if (isAdded)
                {
                    TempData["Message"] = "Question asked successfully";
                }
                else
                {
                    TempData["Message"] = "Ooops something went wrong";
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("AskQuestion", "Questions");
        }

        [CustomAuthorize(AllowAccessToUser = true)]
        [HttpGet]
        public ActionResult ViewMyQuestions()
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            User userDb = unitOfWork.UserRepository.GetFirst(x => x.Id == LoginUserSession.Current.UserID);
            List<ViewQuestionViewModel> viewQuestionViewModel = new List<ViewQuestionViewModel>();
            userDb.Questions.Where(x => x.IsDeleted == false).ToList().ForEach(x => viewQuestionViewModel.Add(new ViewQuestionViewModel(x)));
            return View(viewQuestionViewModel);
        }


        [HttpGet]
        public ActionResult ViewAllQuestions()
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            List<ViewQuestionViewModel> allQuestionsViewModel = new List<ViewQuestionViewModel>();
            unitOfWork.QuestionRepository.GetAll(x => x.IsDeleted == false).ForEach(x => allQuestionsViewModel.Add(new ViewQuestionViewModel(x)));
            return View(allQuestionsViewModel);
        }

        [HttpGet]
        [CustomAuthorize(AllowAccessToUser = true)]
        public ActionResult QuestionDetails(int id)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            Question questionDb = unitOfWork.QuestionRepository.GetFirst(x => x.Id == id);
            if (questionDb == null)
            {
                ModelState.AddModelError("", "Question not found");
                TempData["ErrorMessage"] = "Question not found";
                return RedirectToAction("Index", "Home");

            }
            if (ModelState.IsValid)
            {
                ViewQuestionViewModel viewQuestionViewModel = new ViewQuestionViewModel(questionDb);
                viewQuestionViewModel.AskedBy = questionDb.User.Username;
                List<UserCommentViewModel> usersCommentsViewModel = GetUsersComments(id);
                List<Comment> commentsDb = unitOfWork.CommentRepository.GetAll();

                for (int i = 0; i < usersCommentsViewModel.Count; i++)
                {
                    if (usersCommentsViewModel[i].ParentCommentId == "")
                    {
                        usersCommentsViewModel[i].CommentTo = questionDb.User.Username;
                    }
                    else
                    {
                        usersCommentsViewModel[i].CommentTo = commentsDb.Find(x => x.Id == Int32.Parse(usersCommentsViewModel[i].ParentCommentId)).User.Username;
                    }
                }

                UserCommentsAndQuestionViewModel userCommentsAndQuestionViewModel = new UserCommentsAndQuestionViewModel();
                userCommentsAndQuestionViewModel.ViewQuestionViewModel = viewQuestionViewModel;
                userCommentsAndQuestionViewModel.UsersCommentsViewModel = usersCommentsViewModel;
                return View(userCommentsAndQuestionViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        #region Helper methods
        public List<UserCommentViewModel> GetUsersComments(int id)
        {
            using (SqlConnection cnn = new SqlConnection(Constants.ConnectionString))
            {
                List<UserCommentViewModel> usersCommentsViewModel = new List<UserCommentViewModel>();
                cnn.Open();
                string query = @"WITH UpperHierarchy (CommentId, QuestionId, CommentText, ParentCommentId, DateCommented, UserId, HierarchyOrder, 
                     lineage) 
                     AS (SELECT com.Id,
				                com.QuestionId, 
                                com.CommentText, 
                                com.ParentCommentId,
				                com.DateCommented,
				                com.UserId,
                                0                          AS HierarchyOrder, 
                                Cast ('/' AS VARCHAR(255)) AS Lineage 
                         FROM   Comments AS com 
                         WHERE  com.ParentCommentId IS NULL AND IsDeleted=0
                         UNION ALL
                         (SELECT com.Id,
				                com.QuestionId,
                                com.CommentText, 
                                com.ParentCommentId,
				                com.DateCommented,
				                com.UserId,
                                HierarchyOrder + 1, 
                                Cast(lineage + Ltrim(Str(com.ParentCommentId, 6, 0)) 
                                     + '/' AS VARCHAR(255)) 
                         FROM   Comments AS com 
                                INNER JOIN UpperHierarchy AS parent 
                                        ON com.ParentCommentId = parent.CommentId
						                WHERE com.IsDeleted=0))
						
                SELECT CommentTextFormatted, DateCommented, U.Username, ParentCommentId, Com.CommentId, lineage, Com.UserId
                FROM Questions AS Q
                INNER JOIN 
	                (SELECT Space(HierarchyOrder*5) + CommentText AS CommentTextFormatted, CommentId, QuestionId, ParentCommentId, DateCommented, UserId, lineage
	                FROM   UpperHierarchy) AS Com
                ON Com.QuestionId=Q.Id
                INNER JOIN Users AS U
                ON U.Id=Com.UserId
                WHERE Q.Id=" + id +
                "ORDER  BY lineage + Ltrim(Str(CommentId, 6, 0))";

                using (SqlCommand command = new SqlCommand(query, cnn))
                {

                    SqlDataReader commandResult = command.ExecuteReader();
                    while (commandResult.Read())
                    {
                        usersCommentsViewModel.Add(new UserCommentViewModel()
                        {
                            Lineage = Convert.ToString(commandResult["lineage"]),
                            CommentId = Convert.ToInt32(commandResult["CommentId"]),
                            ParentCommentId = Convert.ToString(commandResult["ParentCommentId"]),
                            CommentedTextFormatted = Convert.ToString(commandResult["CommentTextFormatted"]),
                            DateCommented = Convert.ToString(commandResult["DateCommented"]),
                            Username = Convert.ToString(commandResult["Username"]),
                            UserId = Convert.ToInt32(commandResult["UserId"])
                        });

                    }
                }
                return usersCommentsViewModel;
            }
        }


        public int MarkQuestionAsDeleted(int id)
        {
            using (SqlConnection cnn = new SqlConnection(Constants.ConnectionString))
            {
                string query = "EXEC udp_MarkQuestionAsDeleted @QuestionId = @id";
                cnn.Open();
                using (SqlCommand command = new SqlCommand(query, cnn))
                {
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                    command.Parameters[0].Value = id;
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int MarkCommentAsDeleted(int id)
        {

            using (SqlConnection cnn = new SqlConnection(Constants.ConnectionString))
            {
                string query = @"WITH UpperHierarchy (Id, QuestionId, CommentText, ParentCommentId, DateCommented, UserId, IsDeleted, HierarchyOrder, 
             lineage) 
             AS (SELECT com.Id,
				        com.QuestionId, 
                        com.CommentText, 
                        com.ParentCommentId,
				        com.DateCommented,
				        com.UserId,
				        com.IsDeleted,
                        0                          AS HierarchyOrder, 
                        Cast ('/' AS VARCHAR(255)) AS Lineage 
                 FROM   Comments AS com 
                 WHERE  com.Id=@id
                 UNION ALL
                 (SELECT com.Id,
				        com.QuestionId,
                        com.CommentText, 
                        com.ParentCommentId,
				        com.DateCommented,
				        com.UserId,
				        com.IsDeleted,
                        HierarchyOrder + 1, 
                        Cast(lineage + Ltrim(Str(com.ParentCommentId, 6, 0)) 
                             + '/' AS VARCHAR(255)) 
                 FROM   Comments AS com 
                        INNER JOIN UpperHierarchy AS parent 
                                ON com.ParentCommentId = parent.Id))

				        UPDATE Comments
				        SET IsDeleted=1
				        WHERE Id IN (SELECT Id FROM UpperHierarchy)";
                cnn.Open();
                using (SqlCommand command = new SqlCommand(query, cnn))
                {
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                    command.Parameters[0].Value = id;
                    return command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        [HttpPost]
        [CustomAuthorize(AllowAccessToUser = true)]
        public ActionResult CommentQuestion(CommentQuestionViewModel commentQuestionViewModel)
        {
            if (string.IsNullOrWhiteSpace(commentQuestionViewModel.CommentText))
            {
                ModelState.AddModelError("", "Please enter a valid comment");
                TempData["ErrorMessage"] = "Please enter a valid comment";
                return RedirectToAction("CommentQuestion", commentQuestionViewModel.Id);
            }
            UnitOfWork unitOfWork = new UnitOfWork();
            Question questionDb = unitOfWork.QuestionRepository.GetFirst(x => x.Id == commentQuestionViewModel.Id);
            if (questionDb == null)
            {
                ModelState.AddModelError("", "Question not found");
                TempData["ErrorMessage"] = "Question not found";
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                Comment comment = new Comment();
                comment.DateCommented = DateTime.Now;
                comment.UserId = LoginUserSession.Current.UserID;
                comment.CommentText = commentQuestionViewModel.CommentText;
                comment.IsDeleted = false;
                questionDb.Comments.Add(comment);
                bool isSaved = unitOfWork.Save() > 0;
                if (isSaved)
                {
                    TempData["Message"] = "Question answered successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ooops something went wrong";
                }
            }
            return RedirectToAction("QuestionDetails", new { id = questionDb.Id });
        }

        [HttpPost]
        public ActionResult CommentAComment(CommentACommentViewModel commentACommentViewModel)
        {
            if (commentACommentViewModel == null)
            {
                ModelState.AddModelError("", "No data inputted");
                TempData["ErrorMessage"] = "No data inputted";
            }
            if (commentACommentViewModel.QuestionId == 0)
            {
                ModelState.AddModelError("", "Question not found");
                TempData["ErrorMessage"] = "Question not found";
            }
            if (commentACommentViewModel.CommentId == 0)
            {
                ModelState.AddModelError("", "Comment not found");
                TempData["ErrorMessage"] = "Comment not found";
            }
            if (string.IsNullOrWhiteSpace(commentACommentViewModel.CommentText))
            {
                ModelState.AddModelError("", "Please enter a valid comment");
                TempData["ErrorMessage"] = "Please enter a valid comment";
            }
            if (ModelState.IsValid)
            {
                UnitOfWork unitOfWork = new UnitOfWork();
                Comment commentDb = unitOfWork.CommentRepository.GetFirst(x => x.Id == commentACommentViewModel.CommentId);
                commentDb.Comments1.Add(new Comment()
                {
                    DateCommented = DateTime.Now,
                    IsDeleted = false,
                    QuestionId = commentACommentViewModel.QuestionId,
                    CommentText = commentACommentViewModel.CommentText,
                    UserId = LoginUserSession.Current.UserID
                });
                bool isSaved = unitOfWork.Save() > 0;
                if (isSaved)
                {
                    TempData["Message"] = "Comment commented successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ooops something went wrong";
                }
                return RedirectToAction("QuestionDetails", new { id = commentACommentViewModel.QuestionId });
            }
            return RedirectToAction("Index", "Home");
        }


        [CustomAuthorize(AllowAccessToUser = true)]
        public ActionResult DeleteQuestion(int id)
        {
            bool isSaved = MarkQuestionAsDeleted(id) > 0;
            if (isSaved)
            {
                TempData["Message"] = "Question deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Ooops something went wrong";
            }
            return RedirectToAction("Index", "Home");
        }


        [CustomAuthorize(AllowAccessToUser = true)]
        public ActionResult DeleteComment(int id)
        {
            bool isSaved = MarkCommentAsDeleted(id) > 0;

            if (isSaved)
            {
                TempData["Message"] = "Comment deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Ooops something went wrong";
            }
            return RedirectToAction("Index", "Home");
        }

        [CustomAuthorize(AllowAccessToUser = true)]
        [HttpGet]
        public ActionResult EditQuestion(int id)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            User userDb = unitOfWork.UserRepository.GetFirst(x => x.Id == LoginUserSession.Current.UserID);

            if (!userDb.Questions.Any(x => x.Id == id && x.IsDeleted == false))
            {
                ModelState.AddModelError("", "Question not found");
                TempData["ErrorMessage"] = "Question not found";
            }

            if (ModelState.IsValid)
            {
                Question questionDb = userDb.Questions.FirstOrDefault(x => x.Id == id);
                AskQuestionViewModel editQuestionViewModel = new AskQuestionViewModel(questionDb.Categories.ToList()); //already checked categories
                editQuestionViewModel.Title = questionDb.Title;
                editQuestionViewModel.QuestionText = questionDb.QuestionText;
                editQuestionViewModel.QuestionId = questionDb.Id;
                unitOfWork.CategoryRepository.GetAll().Where(x => !editQuestionViewModel.CheckedCategories.Any(c => c.Id == x.Id)).ToList().ForEach(x => editQuestionViewModel.AllCategories.Add(new CategoryViewModel(x)));
                return View(editQuestionViewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        [CustomAuthorize(AllowAccessToUser = true)]
        [HttpPost]
        public ActionResult EditQuestion(AskQuestionViewModel editQuestionViewModel)
        {
            if (editQuestionViewModel.QuestionId == 0)
            {
                TempData["ErrorMessage"] = "Question not found";
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrWhiteSpace(editQuestionViewModel.Title))
            {
                ModelState.AddModelError("", "Please enter a valid question title");
                TempData["ErrorMessage"] = "Please enter a valid question title";
            }

            if (string.IsNullOrWhiteSpace(editQuestionViewModel.QuestionText))
            {
                ModelState.AddModelError("", "Please enter a valid question");
                TempData["ErrorMessage"] = "Please enter a valid question";
            }

            if (editQuestionViewModel.ChosenCategoriesIds.Count == 0)
            {
                ModelState.AddModelError("", "Please choose at least 1 category");
                TempData["ErrorMessage"] = "Please choose at least 1 category";
            }

            if (ModelState.IsValid)
            {
                UnitOfWork unitOfWork = new UnitOfWork();
                Question questionDb = unitOfWork.QuestionRepository.GetFirst(x => x.Id == editQuestionViewModel.QuestionId);
                questionDb.Title = editQuestionViewModel.Title;
                questionDb.QuestionText = editQuestionViewModel.QuestionText;
                questionDb.UserId = LoginUserSession.Current.UserID;
                questionDb.IsDeleted = false;
                questionDb.Categories.Clear();
                editQuestionViewModel.ChosenCategoriesIds.ForEach(c => questionDb.Categories.Add(unitOfWork.CategoryRepository.GetFirst(x => x.Id == c)));
                bool isAdded = unitOfWork.Save() > 0;
                if (isAdded)
                {
                    TempData["Message"] = "Question edited successfully";
                }
                else
                {
                    TempData["Message"] = "Ooops something went wrong";
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("EditQuestion", editQuestionViewModel.QuestionId);
        }
    }
}