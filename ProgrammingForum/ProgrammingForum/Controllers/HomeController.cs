using DataAccess.Models;
using DataAccess.Repositories;
using ProgrammingForum.Helpers;
using ProgrammingForum.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProgrammingForum.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUser(RegisterUserViewModel registerUserViewModel)
        {
            if (ModelState.IsValid)
            {
                UnitOfWork unitOfWork = new UnitOfWork();
                User existingDbUser = unitOfWork.UserRepository.GetFirst(x => x.Username == registerUserViewModel.Username);
                if (existingDbUser != null)
                {
                    ModelState.AddModelError("", "A user with this username already exists!");
                    return View();
                }

                if (registerUserViewModel.Password != registerUserViewModel.RepeatPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match!");
                    return View();
                }
                User dbUser = new User();
                dbUser.Username = registerUserViewModel.Username;
                dbUser.Password = registerUserViewModel.Password;
                dbUser.IsAdmin = false;
                unitOfWork.UserRepository.Create(dbUser);
                bool isSaved = unitOfWork.Save() > 0;
                if (isSaved)
                {
                    TempData["Message"] = "User was registered successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ooops something went wrong";
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                UnitOfWork unitOfWork = new UnitOfWork();
                User dbUser = unitOfWork.UserRepository.GetFirst(x => x.Username == viewModel.Username && x.Password == viewModel.Password);
                bool isUserExist = dbUser != null;
                if (isUserExist)
                {
                    LoginUserSession.Current.SetCurrentUser(dbUser.Id, dbUser.Username, dbUser.IsAdmin);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username and/or password");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            LoginUserSession.Current.Logout();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult AddCategory(CategoryViewModel addCategoryViewModel)
        {
            if (string.IsNullOrWhiteSpace(addCategoryViewModel.CategoryName))
            {
                ModelState.AddModelError("", "Please enter a valid category name");
                return View();
            }

            if (string.IsNullOrWhiteSpace(addCategoryViewModel.Description))
            {
                ModelState.AddModelError("", "Please enter a valid category description");
                return View();
            }
            if (ModelState.IsValid)
            {
                UnitOfWork unitOfWork = new UnitOfWork();
                Category categoryInput = new Category();
                categoryInput.CategoryName = addCategoryViewModel.CategoryName;
                categoryInput.Description = addCategoryViewModel.Description;
                unitOfWork.CategoryRepository.Save(categoryInput);
                bool isAdded = unitOfWork.Save() > 0;
                if (isAdded)
                {
                    TempData["Message"] = "Category added successfully";
                }
                else
                {
                    TempData["Message"] = "Ooops something went wrong";
                }
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("AddCategory", "Home");
        }
    }
}