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
    public class EvaluationController : BaseController
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

            ResponseModel response = new ResponseModel();
            dynamic data = new ExpandoObject();
            try
            {
                var employee = db.EMPLOYEEs.FirstOrDefault(o => o.EMP_NUM.Equals(user.USER_ID));
                var project_list = db.PROJECT_TASKs
                    .Where(o => o.EMP_NUM == user.USER_ID)
                    .Join(db.PROJECTs,
                            pt => pt.PROJ_CODE,
                            p => p.PROJ_CODE,
                            (pt, p) => pt.PROJ_CODE)
                    .ToList();

                switch (user.USER_POS)
                {
                    case ADMIN:
                        //data.project_list = db.PROJECTs.Select(o => new
                        //{
                        //    o.PROJ_CODE,
                        //    o.PROJ_NAME
                        //}).ToList();
                        //data.evaluations = db.PROJECT_TASKs
                        //    .GroupJoin(db.EVALUATIONs,
                        //            pt => new { PROJ_CODE = pt.PROJ_CODE, EMP_NUM = pt.EMP_NUM },
                        //            eval => new { PROJ_CODE = eval.PROJ_CODE, EMP_NUM = eval.EMP_NUM },
                        //            (pt, eval) => new { pt, eval })
                        //    .SelectMany(
                        //        o => o.eval.DefaultIfEmpty(),
                        //        (pt, eval) => new
                        //        {
                        //            EVAL_NUM = eval == null ? -1 : eval.EVAL_NUM,
                        //            pt.pt.EMP_NUM,
                        //            pt.pt.EMPLOYEE.EMP_NAME,
                        //            pt.pt.PROJ_CODE,
                        //            pt.pt.PROJECT.PROJ_NAME,
                        //            EVAL_HRDWRK = eval == null ? -1 : eval.EVAL_HRDWRK,
                        //            EVAL_FRDLY = eval == null ? -1 : eval.EVAL_FRDLY,
                        //            EVAL_CRTV = eval == null ? -1 : eval.EVAL_CRTV,
                        //            eval.EVAL_CMT
                        //        })
                        //    .Where(o => o.EMP_NUM != o.EVAL_NUM)
                        //    .OrderBy(o => o.PROJ_CODE)
                        //    .Distinct()
                        //    .ToList();
                        data.marker_num = employee.EMP_NUM;
                        data.project_list = db.PROJECTs
                            .Select(o => new
                            {
                                o.PROJ_CODE,
                                o.PROJ_NAME
                            })
                            .Where(o => project_list.Contains(o.PROJ_CODE))
                            .ToList();
                        data.evaluations = db.PROJECT_TASKs
                            .Where(o => project_list.Contains(o.PROJECT.PROJ_CODE))
                            .GroupJoin(db.EVALUATIONs,
                                    pt => new { PROJ_CODE = pt.PROJ_CODE, EMP_NUM = pt.EMP_NUM },
                                    eval => new { PROJ_CODE = eval.PROJ_CODE, EMP_NUM = eval.EMP_NUM },
                                    (pt, eval) => new { pt, eval })
                            .SelectMany(
                                o => o.eval.DefaultIfEmpty(),
                                (pt, eval) => new
                                {
                                    EVAL_NUM = eval == null ? -1 : eval.EVAL_NUM,
                                    pt.pt.EMP_NUM,
                                    pt.pt.EMPLOYEE.EMP_NAME,
                                    pt.pt.PROJ_CODE,
                                    pt.pt.PROJECT.PROJ_NAME,
                                    EVAL_HRDWRK = eval == null ? -1 : eval.EVAL_HRDWRK,
                                    EVAL_FRDLY = eval == null ? -1 : eval.EVAL_FRDLY,
                                    EVAL_CRTV = eval == null ? -1 : eval.EVAL_CRTV,
                                    eval.EVAL_CMT
                                })
                            .Where(o => o.EMP_NUM != o.EVAL_NUM && o.EMP_NUM != user.USER_ID)
                            .OrderBy(o => o.PROJ_CODE)
                            .Distinct()
                            .ToList();
                        break;
                    case MANAGER:
                        data.marker_num = employee.EMP_NUM;
                        data.project_list = db.PROJECTs
                            .Select(o => new
                        {
                            o.PROJ_CODE,
                            o.PROJ_NAME
                        })
                            .Where(o => project_list.Contains(o.PROJ_CODE))
                            .ToList();
                        data.evaluations = db.PROJECT_TASKs
                            .Where(o => project_list.Contains(o.PROJECT.PROJ_CODE))
                            .GroupJoin(db.EVALUATIONs,
                                    pt => new { PROJ_CODE = pt.PROJ_CODE, EMP_NUM = pt.EMP_NUM },
                                    eval => new { PROJ_CODE = eval.PROJ_CODE, EMP_NUM = eval.EMP_NUM },
                                    (pt, eval) => new { pt, eval })
                            .SelectMany(
                                o => o.eval.DefaultIfEmpty(),
                                (pt, eval) => new
                                {
                                    EVAL_NUM = eval == null ? -1 : eval.EVAL_NUM,
                                    pt.pt.EMP_NUM,
                                    pt.pt.EMPLOYEE.EMP_NAME,
                                    pt.pt.PROJ_CODE,
                                    pt.pt.PROJECT.PROJ_NAME,
                                    EVAL_HRDWRK = eval == null ? -1 : eval.EVAL_HRDWRK,
                                    EVAL_FRDLY = eval == null ? -1 : eval.EVAL_FRDLY,
                                    EVAL_CRTV = eval == null ? -1 : eval.EVAL_CRTV,
                                    eval.EVAL_CMT
                                })
                            .Where(o => o.EMP_NUM != o.EVAL_NUM && o.EMP_NUM != user.USER_ID)
                            .OrderBy(o => o.PROJ_CODE)
                            .Distinct()
                            .ToList();
                        break;
                    case EMPLOYEE:
                        data.marker_num = employee.EMP_NUM;
                        data.project_list = db.PROJECTs
                            .Select(o => new
                            {
                                o.PROJ_CODE,
                                o.PROJ_NAME
                            })
                            .Where(o => project_list.Contains(o.PROJ_CODE))
                            .ToList();
                        data.evaluations = db.PROJECT_TASKs
                            .Join(db.PROJECT_TASKs,
                                    pt1 => pt1.PROJ_CODE,
                                    pt2 => pt2.PROJ_CODE,
                                    (pt1, pt2) => new { pt1, pt2 })
                            .GroupJoin(db.EVALUATIONs,
                                    pt => new { pt.pt1.PROJ_CODE, EVAL_NUM = pt.pt2.EMP_NUM, pt.pt1.EMP_NUM },
                                    eval => new { eval.PROJ_CODE, eval.EVAL_NUM, eval.EMP_NUM },
                                    (pt, eval) => new { pt, eval })
                            .SelectMany(o => o.eval.DefaultIfEmpty(),
                                (pt, eval) => new
                                {
                                    pt.pt.pt1.PROJ_CODE,
                                    pt.pt.pt1.EMP_NUM,
                                    EVAL_NUM = pt.pt.pt2.EMP_NUM,
                                    EVAL_NAME = pt.pt.pt2.EMPLOYEE.EMP_NAME,
                                    EVAL_HRDWRK = eval == null ? -1 : eval.EVAL_HRDWRK,
                                    EVAL_FRDLY = eval == null ? -1 : eval.EVAL_FRDLY,
                                    EVAL_CRTV = eval == null ? -1 : eval.EVAL_CRTV,
                                    eval.EVAL_CMT
                                })
                            .Distinct()
                            .Where(o => o.EMP_NUM == user.USER_ID)
                            .ToList();
                        break;
                }

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get evaluations data successfully!";
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
                int eval_num = int.Parse(Request["eval_num"]);
                int emp_num = int.Parse(Request["emp_num"]);
                string proj_code = Request["proj_code"];
                var evaluation = db.EVALUATIONs
                    .Select(o => new
                    {
                        o.EVAL_NUM,
                        o.EMPLOYEE.EMP_NAME,
                        o.EMP_NUM,
                        o.PROJ_CODE,
                        o.PROJECT.PROJ_NAME,
                        o.EVAL_HRDWRK,
                        o.EVAL_FRDLY,
                        o.EVAL_CRTV,
                        o.EVAL_CMT
                    })
                    .FirstOrDefault(o => o.EVAL_NUM == eval_num && o.EMP_NUM == emp_num && o.PROJ_CODE.Equals(proj_code));

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get evaluation data successfully!";
                response.data = JsonConvert.SerializeObject(evaluation);
                return Json(response);
            }
            catch (Exception ex)
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        //public ActionResult Add()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult AddData()
        //{
        //    USER user = ValidateUser();
        //    if (user == null) return Json(StatusUnauthorized());

        //    ResponseModel response = new ResponseModel();
        //    try
        //    {
        //        var data = db.EMPLOYEEs.Select(o => new
        //        {
        //            o.EMP_NUM,
        //            o.EMP_EMAIL,
        //            o.EMP_PHONE,
        //            OPTION = o.EMP_NAME + " #" + o.EMP_NUM
        //        }).ToList();

        //        response.status = ResponseModel.StatusCode.Success;
        //        response.message = "Get data successfully!";
        //        response.data = JsonConvert.SerializeObject(data);
        //        return Json(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.status = ResponseModel.StatusCode.Error;
        //        response.message = "Error";
        //        return Json(response);
        //    }
        //}

        //[HttpPost]
        //public ActionResult AddSubmit()
        //{
        //    USER user = ValidateUser();
        //    if (user == null) return Json(StatusUnauthorized());

        //    ResponseModel response = new ResponseModel();
        //    try
        //    {
        //        string code = Request.Form["code"];
        //        string name = Request.Form["name"];
        //        int emp_num = int.Parse(Request.Form["chairman"]);
        //        string address = Request.Form["address"];

        //        if (code == "" || code == null ||
        //            name == "" || name == null ||
        //            emp_num == 0 ||
        //            address == "" || address == null)
        //        {
        //            response.status = ResponseModel.StatusCode.Error;
        //            response.message = "Invalid value";
        //            return Json(response);
        //        }

        //        if (db.EVALUATIONs.FirstOrDefault(o => o.DEPT_CODE.ToUpper().Equals(code.ToUpper())) != null)
        //        {
        //            response.status = ResponseModel.StatusCode.NotFound;
        //            response.message = "Code is existed";
        //            return Json(response);
        //        }

        //        DEPARTMENT dept = new DEPARTMENT();
        //        dept.DEPT_CODE = code;
        //        dept.DEPT_NAME = name;
        //        dept.EMP_NUM = emp_num;
        //        dept.DEPT_ADDRESS = address;
        //        db.EVALUATIONs.InsertOnSubmit(dept);
        //        db.SubmitChanges();

        //        response.status = ResponseModel.StatusCode.Success;
        //        response.message = "Add department successfully!";
        //        return Json(response);
        //    }
        //    catch
        //    {
        //        response.status = ResponseModel.StatusCode.Error;
        //        response.message = "Error";
        //        return Json(response);
        //    }
        //}

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
                int eval_num = int.Parse(Request["eval_num"]);
                int emp_num = int.Parse(Request["emp_num"]);
                string proj_code = Request["proj_code"];
                var evaluation = db.EVALUATIONs
                    .Select(o => new
                    {
                        o.EVAL_NUM,
                        o.EMPLOYEE1.EMP_NAME,
                        o.EMP_NUM,
                        o.PROJ_CODE,
                        o.PROJECT.PROJ_NAME,
                        o.EVAL_HRDWRK,
                        o.EVAL_FRDLY,
                        o.EVAL_CRTV,
                        o.EVAL_CMT
                    })
                    .FirstOrDefault(o => o.EVAL_NUM == eval_num && o.EMP_NUM == emp_num && o.PROJ_CODE.Equals(proj_code));

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get evaluation data successfully!";
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
                int eval_num = int.Parse(Request.Form["eval_num"]);
                int emp_num = int.Parse(Request.Form["emp_num"]);
                string proj_code = Request.Form["proj_code"];
                int eval_hardwork = int.Parse(Request.Form["eval_hardwork"]);
                int eval_friendly = int.Parse(Request.Form["eval_friendly"]);
                int eval_creative = int.Parse(Request.Form["eval_creative"]);
                string eval_cmt = Request.Form["eval_cmt"];

                var old_eval = db.EVALUATIONs.FirstOrDefault(o => o.EVAL_NUM == eval_num && o.EMP_NUM == emp_num && o.PROJ_CODE.Equals(proj_code));
                old_eval.EVAL_HRDWRK = eval_hardwork;
                old_eval.EVAL_FRDLY = eval_friendly;
                old_eval.EVAL_CRTV = eval_creative;
                old_eval.EVAL_CMT = eval_cmt;
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Update evaluation successfully!";
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
