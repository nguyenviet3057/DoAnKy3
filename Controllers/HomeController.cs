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
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            //Random rand = new Random();
            //List<int> list = new List<int>()
            //{
            //    0, 25, 50, 100
            //};
            //var eval_list = db.EVALUATIONs.ToList();
            //foreach (var item in eval_list)
            //{
            //    item.EVAL_HRDWRK = list[rand.Next(0, 4)];
            //    item.EVAL_FRDLY = list[rand.Next(0, 4)];
            //    item.EVAL_CRTV = list[rand.Next(0, 4)];
            //}
            //db.SubmitChanges();

            ResponseModel response = new ResponseModel();
            try
            {
                dynamic data = new ExpandoObject();
                data.rewarded_employee = null;

                var proj_list = db.PROJECT_TASKs
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .Select(o => o.PROJ_CODE)
                    .ToList();
                data.progress_group = db.PROJECT_TASKs
                    .Where(o => proj_list.Contains(o.PROJ_CODE))
                    .Join(db.PROJECTs,
                        project_task => project_task.PROJ_CODE,
                        project => project.PROJ_CODE,
                        (project_task, project) => new { project_task, project })
                    .GroupBy(o => new { o.project.PROJ_CODE, o.project.PROJ_NAME })
                    .Select(o => new
                    {
                        o.Key.PROJ_CODE,
                        o.Key.PROJ_NAME,
                        AVG_PROG = o.Average(group => group.project_task.PROJTSK_PROG)
                    });
                data.progress_self = db.PROJECT_TASKs
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .Join(db.PROJECTs,
                        project_task => project_task.PROJ_CODE,
                        project => project.PROJ_CODE,
                        (project_task, project) => new { project_task, project })
                    .GroupBy(o => new { o.project.EMP_NUM, o.project.PROJ_CODE, o.project.PROJ_NAME })
                    .Select(o => new
                    {
                        o.Key.PROJ_CODE,
                        o.Key.PROJ_NAME,
                        AVG_PROG = o.Average(group => group.project_task.PROJTSK_PROG)
                    });
                data.evaluation_group = db.EVALUATIONs
                    .Where(o => proj_list.Contains(o.PROJ_CODE))
                    .Join(db.PROJECTs,
                        evaluation => evaluation.PROJ_CODE,
                        project => project.PROJ_CODE,
                        (evaluation, project) => new { evaluation, project })
                    .GroupBy(o => new { o.project.PROJ_CODE, o.project.PROJ_NAME })
                    .Select(o => new
                    {
                        o.Key.PROJ_NAME,
                        AVG_EVAL_HW = o.Average(group => group.evaluation.EVAL_HRDWRK),
                        AVG_EVAL_FR = o.Average(group => group.evaluation.EVAL_FRDLY),
                        AVG_EVAL_CR = o.Average(group => group.evaluation.EVAL_CRTV)
                    });
                data.evaluation_self = db.EVALUATIONs
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .Join(db.PROJECTs,
                        evaluation => evaluation.PROJ_CODE,
                        project => project.PROJ_CODE,
                        (evaluation, project) => new { evaluation, project })
                    .GroupBy(o => new { o.project.PROJ_CODE, o.project.PROJ_NAME })
                    .Select(o => new
                    {
                        o.Key.PROJ_NAME,
                        AVG_EVAL_HW = o.Average(group => group.evaluation.EVAL_HRDWRK),
                        AVG_EVAL_FR = o.Average(group => group.evaluation.EVAL_FRDLY),
                        AVG_EVAL_CR = o.Average(group => group.evaluation.EVAL_CRTV)
                    });
                data.team_project = db.PROJECT_TASKs
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .GroupBy(o => o.PROJ_CODE)
                    .Count();
                data.total_employee = db.USERs.Count();
                var time_keeps = db.TIME_KEEPs
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .OrderBy(o => o.TIME_CLK_IN)
                    .Select(o => new
                    {
                        o.TIME_DATE,
                        o.TIME_CLK_IN,
                        o.TIME_CLK_OUT,
                        o.TIME_ABST
                    })
                    .Take(5).ToList();
                data.time_keeping = time_keeps.Select(o => new
                {
                    description = (o.TIME_ABST == 1 && o.TIME_CLK_IN != null && o.TIME_CLK_OUT != null) ? o.TIME_DATE.ToString("dd MMM") + " " + (o.TIME_CLK_IN == null ? "" : o.TIME_CLK_IN.Value.ToString("h':'m")) + " - " + (o.TIME_CLK_OUT == null ? "" : o.TIME_CLK_OUT.Value.ToString("h':'m")) : (o.TIME_ABST == 0 ? "Permitted" : "Not permitted"),
                    absent_status = (o.TIME_ABST == 1) ? "Presented" : "Absent"
                });

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get dashboard data success!";
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
    }
}