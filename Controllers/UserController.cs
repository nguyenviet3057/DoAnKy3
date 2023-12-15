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
    public class UserController : BaseController
    {
        public ActionResult Details()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DetailsData()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();

            try
            {
                dynamic data = new ExpandoObject();
                var employee = db.EMPLOYEEs
                    .Select(o => new
                    {
                        o.EMP_NUM,
                        o.EMP_NAME,
                        o.EMP_GENDER,
                        EMP_DOB = o.EMP_DOB.Year + "/" + o.EMP_DOB.Day + "/" + o.EMP_DOB.Year,
                        o.EMP_ADDRESS,
                        o.EMP_ID,
                        o.EMP_PLACE_OG,
                        o.EMP_PHONE,
                        o.EMP_EMAIL,
                        o.EMP_REL,
                        o.USER.USER_POS,
                        o.DEPT_CODE
                    })
                    .FirstOrDefault(o => o.EMP_NUM == user.USER_ID);
                var department = db.DEPARTMENTs
                    .Select(o => new
                    {
                        o.DEPT_CODE,
                        o.DEPT_NAME
                    })
                    .FirstOrDefault(o => o.DEPT_CODE.ToUpper().Equals(employee.DEPT_CODE.ToUpper()));
                var degrees = db.DEGREE_CONFIRMs
                    .Select(o => new
                    {
                        o.EMP_NUM,
                        o.DEG_CODE,
                        o.DEGREE.DEG_NAME
                    })
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .ToList();
                var contract = db.CONTRACTs
                    .Select(o => new
                    {
                        o.EMP_NUM,
                        o.CONTR_NUM
                    })
                    .FirstOrDefault(o => o.EMP_NUM == user.USER_ID);

                data.employee = employee;
                data.department = department;
                data.degrees = degrees;
                data.contract = contract;

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get user data successfully!";
                response.data = JsonConvert.SerializeObject(data);
                return Json(response);
            }
            catch (Exception ex)
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }
        
        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult EditData()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                dynamic data = new ExpandoObject();
                var employee = db.EMPLOYEEs
                    .Select(o => new
                    {
                        o.EMP_NUM,
                        o.EMP_NAME,
                        o.EMP_GENDER,
                        EMP_DOB = o.EMP_DOB.Month + "/" + o.EMP_DOB.Day + "/" + o.EMP_DOB.Year,
                        o.EMP_ADDRESS,
                        o.EMP_ID,
                        o.EMP_PLACE_OG,
                        o.EMP_PHONE,
                        o.EMP_EMAIL,
                        o.EMP_REL,
                        o.USER.USER_POS,
                        o.DEPT_CODE
                    })
                    .FirstOrDefault(o => o.EMP_NUM == user.USER_ID);
                var department = db.DEPARTMENTs
                    .Select(o => new
                    {
                        o.DEPT_CODE,
                        o.DEPT_NAME
                    })
                    .ToList();
                var degree_confirm = db.DEGREE_CONFIRMs
                    .Select(o => new
                    {
                        o.DEG_CODE,
                        o.EMP_NUM
                    })
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .ToList();
                var degrees = db.DEGREEs
                    .Select(o => new
                    {
                        o.DEG_CODE,
                        o.DEG_NAME
                    })
                    .ToList();
                var contract = db.CONTRACTs
                    .Select(o => new
                    {
                        o.EMP_NUM,
                        o.CONTR_NUM
                    })
                    .FirstOrDefault(o => o.EMP_NUM == user.USER_ID);
                var contracts = db.CONTRACTs
                    .Select(o => new
                    {
                        o.EMP_NUM,
                        o.CONTR_NUM
                    })
                    .Where(o => o.EMP_NUM == null || o.EMP_NUM == user.USER_ID)
                    .ToList();

                data.employee = employee;
                data.department = department;
                data.degree_confirm = degree_confirm;
                data.degrees = degrees;
                data.contract = contract;
                data.contracts = contracts;

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get user data successfully!";
                response.data = JsonConvert.SerializeObject(data);
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
        public ActionResult EditSubmit()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                string name = Request.Form["name"];
                string gender = Request.Form["gender"];
                DateTime dob = DateTime.Parse(Request.Form["dob"]);
                string address = Request.Form["address"];
                string place_og = Request.Form["place_og"];
                string religion = Request.Form["religion"];
                string email = Request.Form["email"];
                string phone = Request.Form["phone"];

                if (db.EMPLOYEEs.FirstOrDefault(o => o.EMP_NUM != user.USER_ID && (o.EMP_EMAIL == email || o.EMP_PHONE == phone)) != null)
                {
                    response.status = ResponseModel.StatusCode.NotFound;
                    response.message = "Email or phone is already existed!";
                    return Json(response);
                }

                var old_emp = db.EMPLOYEEs.FirstOrDefault(o => o.EMP_NUM == user.USER_ID);
                old_emp.EMP_NAME = name;
                old_emp.EMP_GENDER = gender;
                old_emp.EMP_DOB = dob;
                old_emp.EMP_ADDRESS = address;
                old_emp.EMP_PLACE_OG = place_og;
                old_emp.EMP_REL = religion;

                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Update user successfully!";
                return Json(response);
            }
            catch
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }
    }
}