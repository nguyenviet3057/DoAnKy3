using DoAnKy3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DoAnKy3.Controllers
{
    public class ProjectController : BaseController
    {
        // GET: Department
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

                switch (user.USER_POS)
                {
                    case ADMIN:
                        data.projects = db.PROJECTs.Select(o => new
                        {
                            o.PROJ_CODE,
                            o.PROJ_NAME,
                            o.EMPLOYEE.EMP_NAME
                        }).ToList();
                        break;
                    case MANAGER:
                        data.projects = db.PROJECT_TASKs
                            .Where(o => o.EMP_NUM.Equals(employee.EMP_NUM))
                            .Join(db.PROJECTs, tsk => tsk.PROJ_CODE, proj => proj.PROJ_CODE, (tsk, proj) => new
                            {
                                proj.PROJ_CODE,
                                proj.PROJ_NAME,
                                proj.EMPLOYEE.EMP_NAME
                            }).ToList();
                        break;
                    case EMPLOYEE:
                        data.projects = db.PROJECT_TASKs
                            .Where(o => o.EMP_NUM.Equals(employee.EMP_NUM))
                            .Join(db.PROJECTs, tsk => tsk.PROJ_CODE, proj => proj.PROJ_CODE, (tsk, proj) => new
                            {
                                proj.PROJ_CODE,
                                proj.PROJ_NAME,
                                proj.EMPLOYEE.EMP_NAME
                            }).Distinct().ToList();
                        break;
                }

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get project data successfully!";
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

        // GET: Department/Details?deptcode=
        public ActionResult Details()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DetailsData()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            string proj_code = Request["proj_code"];
            ResponseModel response = new ResponseModel();

            try
            {
                var project = db.PROJECTs
                    .Where(o => o.PROJ_CODE.ToLower().Equals(proj_code.ToLower()))
                    .Join(db.EMPLOYEEs, proj => proj.EMP_NUM, emp => emp.EMP_NUM, (proj, emp) => new
                    {
                        proj.PROJ_CODE,
                        proj.PROJ_NAME,
                        LEADER_NAME = emp.EMP_NAME,
                        LEADER_ID = emp.EMP_NUM,
                        LEADER_EMAIL = emp.EMP_EMAIL,
                        LEADER_PHONE = emp.EMP_PHONE,
                        proj.PROJ_DESC,
                        PROJDEPT_NAME = emp.DEPARTMENT.DEPT_NAME
                    })
                    .FirstOrDefault();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get project data successfully!";
                response.data = JsonConvert.SerializeObject(project);
                return Json(response);
            }
            catch (Exception ex)
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        // GET: Department/Add
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddData()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                var data = db.EMPLOYEEs.Select(o => new
                {
                    o.EMP_NUM,
                    o.EMP_EMAIL,
                    o.EMP_PHONE,
                    OPTION = o.EMP_NAME + " #" + o.EMP_NUM
                }).ToList();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get data successfully!";
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

        // POST: Department/AddSubmit
        [HttpPost]
        public ActionResult AddSubmit()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                string code = Request.Form["code"];
                string name = Request.Form["name"];
                int emp_num = int.Parse(Request.Form["leader"]);
                string desc = Request.Form["desc"];

                if (code == "" || code == null ||
                    name == "" || name == null ||
                    emp_num == 0 ||
                    desc == "" || desc == null)
                {
                    response.status = ResponseModel.StatusCode.Error;
                    response.message = "Invalid value";
                    return Json(response);
                }

                if (db.PROJECTs.FirstOrDefault(o => o.PROJ_CODE.ToUpper().Equals(code.ToUpper())) != null)
                {
                    response.status = ResponseModel.StatusCode.NotFound;
                    response.message = "Code existed";
                    return Json(response);
                }

                PROJECT proj = new PROJECT();
                proj.PROJ_CODE = code;
                proj.PROJ_NAME = name;
                proj.EMP_NUM = emp_num;
                proj.PROJ_DESC = desc;
                db.PROJECTs.InsertOnSubmit(proj);
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Add project successfully!";
                return Json(response);
            }
            catch
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        // GET: Department/Edit?deptcode=
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult EditData()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            string proj_code = Request["proj_code"];
            ResponseModel response = new ResponseModel();

            try
            {
                dynamic data = new ExpandoObject();
                data.project = db.PROJECTs
                    .Select(o => new
                    {
                        o.PROJ_CODE,
                        o.PROJ_NAME,
                        o.EMP_NUM,
                        LEADER_NAME = o.EMPLOYEE.EMP_NAME + " #" + o.EMP_NUM,
                        LEADER_EMAIL = o.EMPLOYEE.EMP_EMAIL,
                        LEADER_PHONE = o.EMPLOYEE.EMP_PHONE,
                        o.PROJ_DESC
                    })
                    .FirstOrDefault(o => o.PROJ_CODE.ToLower().Equals(proj_code.ToLower()));
                data.emp_list = db.EMPLOYEEs.Select(o => new
                {
                    o.EMP_NUM,
                    o.EMP_EMAIL,
                    o.EMP_PHONE,
                    OPTION = o.EMP_NAME + " #" + o.EMP_NUM
                }).ToList();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get project data successfully!";
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

        // POST: Department/Edit/5
        [HttpPost]
        public ActionResult EditSubmit()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            string code = Request.Form["code"];
            string name = Request.Form["name"];
            string leader = Request.Form["leader"];
            string desc = Request.Form["desc"];

            ResponseModel response = new ResponseModel();
            try
            {
                var old_proj = db.PROJECTs.FirstOrDefault(o => o.PROJ_CODE.ToUpper().Equals(code.ToUpper()));
                old_proj.PROJ_NAME = name;
                old_proj.EMP_NUM = int.Parse(leader);
                old_proj.PROJ_DESC = desc;
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Update project successfully!";
                return Json(response);
            }
            catch
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        // POST: Department/DeleteSubmit
        [HttpPost]
        public ActionResult DeleteSubmit()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                string proj_code = Request.Form["proj_code"];
                var old_proj = db.PROJECTs.FirstOrDefault(o => o.PROJ_CODE.ToUpper().Equals(proj_code.ToUpper()));

                db.PROJECTs.DeleteOnSubmit(old_proj);
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Delete project successfully!";
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
