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
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {


            return View();
        }

        [HttpPost]
        public ActionResult GetData()
        {
            string token = Request["access_token"];
            var result = db.USERs.Where(o => o.USER_TOKEN == token).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var response = new ResponseModel();
            dynamic data = new ExpandoObject();
            data.rewarded_employee = null;
            data.average_evaluation = null;
            data.self_evaluation = null;
            data.total_project = null;
            data.total_employee = db.USERs.Count();
            data.time_keeping = db.TIME_KEEPs
                .Where(o => o.EMP_NUM == result.USER_ID)
                .OrderBy(o => o.TIME_CLK_IN)
                .Select(o => new
                {
                    date = o.TIME_DATE,
                    check_in = o.TIME_CLK_IN,
                    check_out = o.TIME_CLK_OUT,
                    is_absent = o.TIME_ABST
                })
                .Take(5);
            data.user_data = db.USERs
                .Join(db.DEPARTMENTs,
                        user => user.USER_ID,
                        dept => dept.EMP_NUM,
                        (user, dept) => new { user, dept })
                .Join(db.EMPLOYEEs,
                        user_dept => user_dept.user.USER_ID,
                        emp => emp.EMP_NUM,
                        (user_dept, emp) => new { user_dept, emp })
                .Where(o => o.user_dept.user.USER_ID == result.USER_ID)
                .Select(o => new
                {
                    name = o.emp.EMP_NAME,
                    department = o.user_dept.dept.DEPT_NAME
                }).FirstOrDefault();

            response.status = ResponseModel.StatusCode.Success;
            response.message = "Get dashboard data success!";
            response.data = JsonConvert.SerializeObject(data);

            return Json(response);
        }
    }
}