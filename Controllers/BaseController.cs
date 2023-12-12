using DoAnKy3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebPages.Instrumentation;

namespace DoAnKy3.Controllers
{
    public class BaseController : Controller
    {
        public ModelClassDataContext db = new ModelClassDataContext();
        public const string ADMIN = "ADMIN";
        public const string MANAGER = "MANAGER";
        public const string EMPLOYEE = "EMPLOYEE";

        public string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public USER ValidateUser()
        {
            string token = Request["access_token"];
            if (token == null)
            {
                return null;
            }
            var result = db.USERs.Where(o => o.USER_TOKEN == token).FirstOrDefault();
            if (result == null)
            {
                return null;
            }
            return result;
        }

        public ResponseModel StatusUnauthorized()
        {
            ResponseModel response = new ResponseModel();
            response.status = ResponseModel.StatusCode.Unauthorized;
            response.message = "Unauthorized";
            return response;
        }
    }
}