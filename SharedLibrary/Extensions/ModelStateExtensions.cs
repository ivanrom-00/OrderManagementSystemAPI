using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedLibrary.Extensions
{
    public static class ModelStateExtensions
    {
        // Extension method for appending ModelState errors in a single string
        public static string GetErrorMessages(this ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return string.Join("; ", errors);
        }
    }
}
