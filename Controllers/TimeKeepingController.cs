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
    public class TimeKeepingController : BaseController
    {
        // GET: TimeKeeping
        public ActionResult Index()
        {
            USER user = ValidateUser();
            if (user == null || user.USER_POS != "MANAGER") return Json(StatusUnauthorized());
            return View();
        }

        [HttpPost]
        public ActionResult GetData()
        {
            USER user = ValidateUser();
            if (user == null || user.USER_POS != "MANAGER") return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            dynamic data = new ExpandoObject();
            try
            {
                var emp_list = db.EMPLOYEEs
                    .Where(o => o.DEPARTMENT.DEPT_CODE == user.EMPLOYEE.DEPT_CODE)
                    .Select(o => o.EMP_NUM)
                    .ToList();

                data.time_keeps = db.TIME_KEEPs
                    .Where(o => emp_list.Contains(o.EMP_NUM) && o.TIME_DATE == DateTime.Now)
                    .Select(o => new
                    {
                        o.TIME_CODE,
                        o.EMPLOYEE.EMP_NAME,
                        TIME_DATE = o.TIME_DATE.Month + "/" + o.TIME_DATE.Day + "/" + o.TIME_DATE.Year,
                        o.TIME_CLK_IN,
                        o.TIME_ABST
                    })
                    .ToList();

                response.status = ResponseModel.StatusCode.Success;
                response.data = JsonConvert.SerializeObject(data);
                response.message = "Get time keeping data successfully!";
                return Json(response);
            }
            catch
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        // GET: TimeKeeping/Checkin
        public ActionResult Checkin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckinData()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            dynamic data = new ExpandoObject();
            try
            {
                data.time_keeps = db.TIME_KEEPs
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .Select(o => new
                    {
                        o.TIME_CODE,
                        TIME_DATE = o.TIME_DATE.Month + "/" + o.TIME_DATE.Day + "/" + o.TIME_DATE.Year,
                        o.TIME_CLK_IN,
                        o.TIME_CLK_OUT,
                        o.TIME_ABST
                    })
                    .ToList();
                data.position = user.USER_POS;

                response.status = ResponseModel.StatusCode.Success;
                response.data = JsonConvert.SerializeObject(data);
                response.message = "Get time keeping data successfully!";
                return Json(response);
            }
            catch
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        [HttpPost]
        public ActionResult CheckinSubmit()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                TIME_KEEP time_keep = new TIME_KEEP();

                var now = DateTime.Now;
                time_keep.TIME_CODE = now.Year.ToString() + now.Month.ToString() + now.Day.ToString() + "-" + user.USER_ID.ToString();
                time_keep.EMP_NUM = user.USER_ID;
                time_keep.TIME_DATE = now;
                time_keep.TIME_CLK_IN = new TimeSpan(0, now.TimeOfDay.Hours, now.TimeOfDay.Minutes, now.TimeOfDay.Seconds);
                time_keep.TIME_ABST = 1;
                db.TIME_KEEPs.InsertOnSubmit(time_keep);

                db.SubmitChanges();

                dynamic data = new ExpandoObject();
                data.TIME_CODE = time_keep.TIME_CODE;
                data.TIME_DATE = time_keep.TIME_DATE.Month + "/" + time_keep.TIME_DATE.Day + "/" + time_keep.TIME_DATE.Year;
                data.TIME_CLK_IN = time_keep.TIME_CLK_IN;

                response.status = ResponseModel.StatusCode.Success;
                response.data = JsonConvert.SerializeObject(data);
                response.message = "Check-in successfully!";
                return Json(response);
            }
            catch
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }
        [HttpPost]
        public ActionResult CheckoutSubmit()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                string time_code = Request.Form["time_code"];
                var time_keep = db.TIME_KEEPs.FirstOrDefault(o => o.EMP_NUM == user.USER_ID && o.TIME_CODE.Equals(time_code));

                if (time_keep == null)
                {
                    response.status = ResponseModel.StatusCode.NotFound;
                    response.message = "Check in not found";
                    return Json(response);
                }

                if (time_keep.TIME_CLK_OUT != null)
                {
                    response.status = ResponseModel.StatusCode.NotFound;
                    response.message = "Checked-out";
                    return Json(response);
                }

                var now = DateTime.Now;
                time_keep.TIME_CLK_OUT = new TimeSpan(0, now.TimeOfDay.Hours, now.TimeOfDay.Minutes, now.TimeOfDay.Seconds);

                db.SubmitChanges();

                dynamic data = new ExpandoObject();
                data.TIME_DATE = time_keep.TIME_DATE.Month + "/" + time_keep.TIME_DATE.Day + "/" + time_keep.TIME_DATE.Year;
                data.TIME_CLK_IN = time_keep.TIME_CLK_IN;

                response.status = ResponseModel.StatusCode.Success;
                response.data = JsonConvert.SerializeObject(data);
                response.message = "Check-out successfully!";
                return Json(response);
            }
            catch
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        // GET: TimeKeeping/Edit?timecode=
        public ActionResult Edit()
        {
            USER user = ValidateUser();
            if (user == null || user.USER_POS != "MANAGER") return Json(StatusUnauthorized());

            return View();
        }

        [HttpPost]
        public ActionResult EditData()
        {
            USER user = ValidateUser();
            if (user == null || user.USER_POS != "MANAGER") return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            dynamic data = new ExpandoObject();
            try
            {
                string time_code = Request.Form["time_code"];
                data.time_keep = db.TIME_KEEPs
                    .Where(o => o.TIME_CODE == time_code)
                    .Select(o => new
                    {
                        TIME_DATE = o.TIME_DATE.Month + "/" + o.TIME_DATE.Day + "/" + o.TIME_DATE.Year,
                        o.TIME_CLK_IN,
                        o.TIME_ABST,
                        o.EMP_NUM,
                        o.EMPLOYEE.EMP_NAME
                    })
                    .FirstOrDefault();

                response.status = ResponseModel.StatusCode.Success;
                response.data = JsonConvert.SerializeObject(data);
                response.message = "Get data successfully!";
                return Json(response);
            }
            catch
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
            if (user == null || user.USER_POS != "MANAGER") return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                string time_code = Request.Form["time_code"];
                DateTime date = DateTime.Parse(Request.Form["date"]);
                TimeSpan clk_in = TimeSpan.Parse(Request.Form["clk_in"]);
                int status = int.Parse(Request.Form["status"]);

                var old_time_keep = db.TIME_KEEPs.FirstOrDefault(o => o.TIME_CODE == time_code);
                old_time_keep.TIME_CLK_IN = clk_in;
                old_time_keep.TIME_ABST = status;
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Update successfully!";
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
