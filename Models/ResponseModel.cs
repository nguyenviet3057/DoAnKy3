using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnKy3.Models
{
    public class ResponseModel
    {
        public enum StatusCode 
        { 
            Success = 1, 
            Error = 0, 
            NotFound = 2, 
            Unauthorized = 3
        }
        public StatusCode status { get; set; }
        public string message { get; set; }
        public string data { get; set; }

        public ResponseModel() 
        {
            this.message = null;
            this.data = null;
        }
        public ResponseModel(StatusCode status, string message, string data)
        {
            this.status = status;
            this.message = message;
            this.data = data;
        }
    }
}