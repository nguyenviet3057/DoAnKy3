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
            data.progress_group = db.PROJECT_TASKs
                .Join(db.PROJECTs,
                    project_task => project_task.PROJ_CODE,
                    project => project.PROJ_CODE,
                    (project_task, project) => new { project_task, project })
                .GroupBy(o => new { o.project.PROJ_CODE, o.project.PROJ_NAME })
                .Select(o => new
                {
                    PROJ_NAME = o.Key.PROJ_NAME,
                    AVG_PROG = o.Average(group => group.project_task.PROJTSK_PROG),
                });
            data.progress_self = db.PROJECT_TASKs
                .Where(o => o.EMP_NUM == result.USER_ID)
                .Join(db.PROJECTs,
                    project_task => project_task.PROJ_CODE,
                    project => project.PROJ_CODE,
                    (project_task, project) => new { project_task, project })
                .GroupBy(o => new { o.project.PROJ_CODE, o.project.PROJ_NAME })
                .Select(o => new
                {
                    PROJ_NAME = o.Key.PROJ_NAME,
                    AVG_PROG = o.Average(group => group.project_task.PROJTSK_PROG),
                });
            data.evaluation_group = db.EVALUATIONs
                .Join(db.PROJECTs,
                    evaluation => evaluation.PROJ_CODE,
                    project => project.PROJ_CODE,
                    (evaluation, project) => new { evaluation, project })
                .GroupBy(o => new { o.project.PROJ_CODE, o.project.PROJ_NAME })
                .Select(o => new
                {
                    PROJ_NAME = o.Key.PROJ_NAME,
                    AVG_EVAL_HW = o.Average(group => group.evaluation.EVAL_HRDWRK),
                    AVG_EVAL_FR = o.Average(group => group.evaluation.EVAL_FRDLY),
                    AVG_EVAL_CR = o.Average(group => group.evaluation.EVAL_CRTV)
                });
            data.evaluation_self = db.EVALUATIONs
                .Where(o => o.EMP_NUM == result.USER_ID)
                .Join(db.PROJECTs,
                    evaluation => evaluation.PROJ_CODE,
                    project => project.PROJ_CODE,
                    (evaluation, project) => new { evaluation, project })
                .GroupBy(o => new { o.project.PROJ_CODE, o.project.PROJ_NAME })
                .Select(o => new
                {
                    PROJ_NAME = o.Key.PROJ_NAME,
                    AVG_EVAL_HW = o.Average(group => group.evaluation.EVAL_HRDWRK),
                    AVG_EVAL_FR = o.Average(group => group.evaluation.EVAL_FRDLY),
                    AVG_EVAL_CR = o.Average(group => group.evaluation.EVAL_CRTV)
                });
            data.team_project = db.PROJECT_TASKs
                .Where(o => o.EMP_NUM == result.USER_ID)
                .GroupBy(o => o.PROJ_CODE)
                .Count();
            data.total_employee = db.USERs.Count();
            var time_keeps = db.TIME_KEEPs
                .Where(o => o.EMP_NUM == result.USER_ID)
                .OrderBy(o => o.TIME_CLK_IN)
                .Select(o => new
                {
                    TIME_DATE = o.TIME_DATE,
                    TIME_CLK_IN = o.TIME_CLK_IN,
                    TIME_CLK_OUT = o.TIME_CLK_OUT,
                    TIME_ABST = o.TIME_ABST
                })
                .Take(5).ToList();
            data.time_keeping = time_keeps.Select(o => new
            {
                description = (o.TIME_ABST == 1 && o.TIME_CLK_IN != null && o.TIME_CLK_OUT != null) ? o.TIME_DATE.ToString("dd MMM") + " " + (o.TIME_CLK_IN == null ? "" : o.TIME_CLK_IN.Value.ToString("h':'m")) + " - " + (o.TIME_CLK_OUT == null ? "" : o.TIME_CLK_OUT.Value.ToString("h':'m")) : (o.TIME_ABST == 0 ? "Permitted" : "Not permitted"),
                absent_status = (o.TIME_ABST == 1) ? "Presented" : "Absent"
            });
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