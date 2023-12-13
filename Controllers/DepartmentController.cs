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
    public class DepartmentController : BaseController
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
                        data.departments = db.DEPARTMENTs.Select(o => new
                        {
                            o.DEPT_CODE,
                            o.DEPT_NAME,
                            o.EMPLOYEE.EMP_NAME,
                            o.DEPT_ADDRESS,
                            o.DEPT_DESC
                        }).ToList();
                        break;
                    case MANAGER:
                        data.departments = db.DEPARTMENTs.Where(o => o.EMP_NUM.Equals(employee.EMP_NUM)).Select(o => new
                        {
                            o.DEPT_CODE,
                            o.DEPT_NAME,
                            o.EMPLOYEE.EMP_NAME,
                            o.DEPT_ADDRESS,
                            o.DEPT_DESC
                        }).ToList();
                        break;
                    case EMPLOYEE:
                        data.departments = db.DEPARTMENTs.Where(o => o.DEPT_CODE.Equals(employee.DEPT_CODE)).Select(o => new
                        {
                            o.DEPT_CODE,
                            o.DEPT_NAME,
                            o.EMPLOYEE.EMP_NAME,
                            o.DEPT_ADDRESS,
                            o.DEPT_DESC
                        }).ToList();
                        break;
                }

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get department data successfully!";
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

            string dept_code = Request["dept_code"];
            ResponseModel response = new ResponseModel();

            try
            {
                var department = db.DEPARTMENTs
                    .Select(o => new
                    {
                        o.DEPT_CODE,
                        o.DEPT_NAME,
                        CHAIRMAN_NAME = o.EMPLOYEE.EMP_NAME,
                        CHAIRMAN_EMAIL = o.EMPLOYEE.EMP_EMAIL,
                        CHAIRMAN_PHONE = o.EMPLOYEE.EMP_PHONE,
                        o.DEPT_ADDRESS,
                        o.DEPT_DESC
                    })
                    .FirstOrDefault(o => o.DEPT_CODE.ToLower().Equals(dept_code.ToLower()));

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get department data successfully!";
                response.data = JsonConvert.SerializeObject(department);
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
                int emp_num = int.Parse(Request.Form["chairman"]);
                string address = Request.Form["address"];

                if (code == "" || code == null ||
                    name == "" || name == null ||
                    emp_num == 0 ||
                    address == "" || address == null)
                {
                    response.status = ResponseModel.StatusCode.Error;
                    response.message = "Invalid value";
                    return Json(response);
                }

                if (db.DEPARTMENTs.FirstOrDefault(o => o.DEPT_CODE.ToUpper().Equals(code.ToUpper())) != null)
                {
                    response.status = ResponseModel.StatusCode.NotFound;
                    response.message = "Code is existed";
                    return Json(response);
                }

                DEPARTMENT dept = new DEPARTMENT();
                dept.DEPT_CODE = code;
                dept.DEPT_NAME = name;
                dept.EMP_NUM = emp_num;
                dept.DEPT_ADDRESS = address;
                db.DEPARTMENTs.InsertOnSubmit(dept);
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Add department successfully!";
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

            string dept_code = Request["dept_code"];
            ResponseModel response = new ResponseModel();

            try
            {
                dynamic data = new ExpandoObject();
                data.department = db.DEPARTMENTs
                    .Select(o => new
                    {
                        o.DEPT_CODE,
                        o.DEPT_NAME,
                        o.EMP_NUM,
                        CHAIRMAN_NAME = o.EMPLOYEE.EMP_NAME + " #" + o.EMP_NUM,
                        CHAIRMAN_EMAIL = o.EMPLOYEE.EMP_EMAIL,
                        CHAIRMAN_PHONE = o.EMPLOYEE.EMP_PHONE,
                        o.DEPT_ADDRESS,
                        o.DEPT_DESC
                    })
                    .FirstOrDefault(o => o.DEPT_CODE.ToLower().Equals(dept_code.ToLower()));
                data.emp_list = db.EMPLOYEEs.Select(o => new
                {
                    o.EMP_NUM,
                    o.EMP_EMAIL,
                    o.EMP_PHONE,
                    OPTION = o.EMP_NAME + " #" + o.EMP_NUM
                }).ToList();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get department data successfully!";
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
            string chairman = Request.Form["chairman"];
            string address = Request.Form["address"];

            ResponseModel response = new ResponseModel();
            try
            {
                var old_dept = db.DEPARTMENTs.FirstOrDefault(o => o.DEPT_CODE.ToUpper().Equals(code.ToUpper()));
                old_dept.DEPT_NAME = name;
                old_dept.EMP_NUM = int.Parse(chairman);
                old_dept.DEPT_ADDRESS = address;
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Update department successfully!";
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
                string dept_code = Request.Form["dept_code"];
                var old_dept = db.DEPARTMENTs.FirstOrDefault(o => o.DEPT_CODE.ToUpper().Equals(dept_code.ToUpper()));

                db.DEPARTMENTs.DeleteOnSubmit(old_dept);
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Delete department successfully!";
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
