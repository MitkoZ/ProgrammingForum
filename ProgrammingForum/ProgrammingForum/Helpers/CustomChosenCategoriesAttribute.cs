using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProgrammingForum.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomChosenCategoriesAttribute : RequiredAttribute
    {
        public CustomChosenCategoriesAttribute()
        {
            ErrorMessage = "Please check at least 1 category!";
        }

        public override bool IsValid(object value)
        {
            List<int> chosenCategories = (List<int>)value;
            bool result = true;
            if (chosenCategories.Count == 0)
            {
                result = false;
            }
            return result;
        }
    }
}