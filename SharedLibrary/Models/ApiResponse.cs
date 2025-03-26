using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    // Standarized API response model
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }

        public ApiResponse(bool success, int statusCode, string errorMessage, T data)
        {
            Success = success;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            Data = data;
        }
    }
}
