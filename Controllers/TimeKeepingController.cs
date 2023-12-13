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
            return View();
        }

        [HttpPost]
        public ActionResult GetData()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                var time_keeps = db.TIME_KEEPs
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .Select(o => new
                    {
                        TIME_DATE = o.TIME_DATE.Month + "/" + o.TIME_DATE.Day + "/" + o.TIME_DATE.Year,
                        o.TIME_CLK_IN,
                        o.TIME_CLK_OUT,
                        o.TIME_ABST
                    })
                    .ToList();

                response.status = ResponseModel.StatusCode.Success;
                response.data = JsonConvert.SerializeObject(time_keeps);
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
        public ActionResult Checkin()
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

        // GET: TimeKeeping/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TimeKeeping/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TimeKeeping/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TimeKeeping/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TimeKeeping/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TimeKeeping/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TimeKeeping/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
