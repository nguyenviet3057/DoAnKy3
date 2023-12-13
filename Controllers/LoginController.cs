using DoAnKy3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnKy3.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login()
        {
            var response = new ResponseModel();

            try
            {
                int userId = int.Parse(Request["userid"]);
                string password = Request["password"];
                bool remember = bool.Parse(Request["remember"]);

                var result = db.USERs.Where(o => o.USER_ID == userId && o.USER_PSWD == password).FirstOrDefault();
                if (result == null)
                {
                    response.status = ResponseModel.StatusCode.NotFound;
                    response.message = "Account not found!";
                    return Json(response);
                }

                dynamic data = new ExpandoObject();
                data.token = ComputeSha256Hash(userId.ToString() + password + DateTime.Now.Subtract(new DateTime(1970,1,1)).TotalSeconds);
                if (remember) data.expire = 5 * 24 * 3600;
                else data.expire = 24 * 3600;
                var obj = (IDictionary<string, object>)data;

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Login success!";
                response.data = JsonConvert.SerializeObject(obj);

                result.USER_TOKEN = data.token;
                db.SubmitChanges();

                return Json(response);
            }
            catch (Exception ex)
            {
                response.status = ResponseModel.StatusCode.Error;
                //response.message = "Login failed!";
                response.message = ex.Message;
                return Json(response);
            }
        }

        [HttpPost]
        public ActionResult Validate()
        {
            string token = Request["access_token"];

            var response = new ResponseModel();
            try
            {
                var result = db.USERs
                .Join(db.EMPLOYEEs,
                        usr => usr.USER_ID,
                        emp => emp.EMP_NUM,
                        (usr, emp) => new
                        {
                            emp.EMP_NUM,
                            emp.EMP_NAME,
                            usr.USER_POS,
                            usr.USER_TOKEN
                        })
                .Where(o => o.USER_TOKEN == token)
                .Select(o => new
                {
                    name = o.EMP_NAME,
                    position = o.USER_POS
                })
                .FirstOrDefault();

                if (result == null)
                {
                    response.status = ResponseModel.StatusCode.NotFound;
                    response.message = "Token not found!";
                    return Json(response);
                }

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Auto login success!";
                response.data = JsonConvert.SerializeObject(result);
                return Json(response);
            }
            catch (Exception ex)
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            string token = Request["access_token"];
            
            var response = new ResponseModel();
            try
            {
                var result = db.USERs.Where(o => o.USER_TOKEN == token).FirstOrDefault();
                if (result != null)
                {
                    result.USER_TOKEN = null;
                    db.SubmitChanges();
                }
                response.status = ResponseModel.StatusCode.Success;
                response.message = "Logout success!";
                return Json(response);
            }
            catch (Exception ex)
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }
    }
}