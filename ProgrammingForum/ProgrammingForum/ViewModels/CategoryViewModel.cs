using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProgrammingForum.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        [Required]
        public string Description { get; set; }

        public CategoryViewModel(Category categoryModel)
        {
            this.Id = categoryModel.Id;
            this.CategoryName = categoryModel.CategoryName;
            this.Description = categoryModel.Description;
        }
    }
}