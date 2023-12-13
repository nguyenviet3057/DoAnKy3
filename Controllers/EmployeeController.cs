using DoAnKy3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DoAnKy3.Controllers
{
    public class EmployeeController : BaseController
    {
        // GET: Employee
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
                        data.emp_list = db.EMPLOYEEs
                            .Join(db.CONTRACTs,
                                    emp => emp.EMP_NUM,
                                    contr => contr.EMP_NUM,
                                    (emp, contr) => new { emp, contr })
                            .Join(db.TIME_KEEPs,
                                    emp_contr => emp_contr.emp.EMP_NUM,
                                    time => time.EMP_NUM,
                                    (emp_contr, time) => new
                                    {
                                        emp_contr.emp.EMP_NUM,
                                        emp_contr.emp.EMP_ID,
                                        emp_contr.emp.EMP_NAME,
                                        CONTR_SIGN_DATE = emp_contr.contr.CONTR_SIGN_DATE.Month + "/" + emp_contr.contr.CONTR_SIGN_DATE.Day + "/" + emp_contr.contr.CONTR_SIGN_DATE.Year,
                                        time.TIME_ABST
                                    })
                            .ToList();
                        break;
                    case MANAGER:
                        data.emp_list = db.EMPLOYEEs
                            .Join(db.CONTRACTs,
                                    emp => emp.EMP_NUM,
                                    contr => contr.EMP_NUM,
                                    (emp, contr) => new { emp, contr })
                            .Join(db.TIME_KEEPs,
                                    emp_contr => emp_contr.emp.EMP_NUM,
                                    time => time.EMP_NUM,
                                    (emp_contr, time) => new
                                    {
                                        emp_contr.emp.EMP_NUM,
                                        emp_contr.emp.EMP_ID,
                                        emp_contr.emp.EMP_NAME,
                                        CONTR_SIGN_DATE = emp_contr.contr.CONTR_SIGN_DATE.Month + "/" + emp_contr.contr.CONTR_SIGN_DATE.Day + "/" + emp_contr.contr.CONTR_SIGN_DATE.Year,
                                        time.TIME_ABST
                                    })
                            .ToList();
                        break;
                    case EMPLOYEE:
                        return Json(StatusUnauthorized());
                }

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Get employee data successfully!";
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

        // GET: Employee/Details?deptcode=
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
                int emp_num = int.Parse(Request.Form["emp_num"]);
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
                    .FirstOrDefault(o => o.EMP_NUM == emp_num);
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
                    .Where(o => o.EMP_NUM == emp_num)
                    .ToList();
                var contract = db.CONTRACTs
                    .Select(o => new
                    {
                        o.EMP_NUM,
                        o.CONTR_NUM
                    })
                    .FirstOrDefault(o => o.EMP_NUM == emp_num);

                data.employee = employee;
                data.department = department;
                data.degrees = degrees;
                data.contract = contract;

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

        // GET: Employee/Add
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
            dynamic data = new ExpandoObject();
            try
            {
                var departments = db.DEPARTMENTs
                    .Select(o => new
                    {
                        o.DEPT_CODE,
                        o.DEPT_NAME
                    })
                    .ToList();
                var degrees = db.DEGREEs
                    .Select(o => new
                    {
                        o.DEG_CODE,
                        o.DEG_NAME
                    })
                    .ToList();
                var contracts = db.CONTRACTs
                    .Select(o => new
                    {
                        o.EMP_NUM,
                        o.CONTR_NUM
                    })
                    .Where(o => o.EMP_NUM == null)
                    .ToList();

                data.departments = departments;
                data.degrees = degrees;
                data.contracts = contracts;

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

        // POST: Employee/AddSubmit
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

        // GET: Employee/Edit?deptcode=
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
                int emp_num = int.Parse(Request.Form["emp_num"]);
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
                    .FirstOrDefault(o => o.EMP_NUM == emp_num);
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
                    .Where(o => o.EMP_NUM == emp_num)
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
                    .FirstOrDefault(o => o.EMP_NUM == emp_num);

                data.employee = employee;
                data.department = department;
                data.degree_confirm = degree_confirm;
                data.degrees = degrees;
                data.contract = contract;

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

        [HttpPost]
        public ActionResult EditSubmit()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                int emp_num = int.Parse(Request.Form["emp_num"]);
                string name = Request.Form["name"];
                string gender = Request.Form["gender"];
                DateTime dob = DateTime.Parse(Request.Form["dob"]);
                string address = Request.Form["address"];
                string place_og = Request.Form["place_og"];
                string religion = Request.Form["religion"];
                string department = Request.Form["department"];
                List<string> degrees = JsonConvert.DeserializeObject<List<string>>(Request.Form["degrees"]);

                var old_emp = db.EMPLOYEEs.FirstOrDefault(o => o.EMP_NUM == emp_num);
                old_emp.EMP_NAME = name;
                old_emp.EMP_GENDER = gender;
                old_emp.EMP_DOB = dob;
                old_emp.EMP_ADDRESS = address;
                old_emp.EMP_PLACE_OG = place_og;
                old_emp.EMP_REL = religion;
                old_emp.DEPT_CODE = department;

                var old_deg_cf = db.DEGREE_CONFIRMs.Where(o => o.EMP_NUM == emp_num).ToList();
                db.DEGREE_CONFIRMs.DeleteAllOnSubmit(old_deg_cf);
                foreach (string deg in degrees)
                {
                    DEGREE_CONFIRM deg_cf = new DEGREE_CONFIRM();
                    deg_cf.DEG_CODE = deg;
                    deg_cf.EMP_NUM = emp_num;
                    db.DEGREE_CONFIRMs.InsertOnSubmit(deg_cf);
                }
                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Update employee successfully!";
                return Json(response);
            }
            catch
            {
                response.status = ResponseModel.StatusCode.Error;
                response.message = "Error";
                return Json(response);
            }
        }

        // POST: Employee/DeleteSubmit
        [HttpPost]
        public ActionResult DeleteSubmit()
        {
            USER user = ValidateUser();
            if (user == null) return Json(StatusUnauthorized());

            ResponseModel response = new ResponseModel();
            try
            {
                int emp_num = int.Parse(Request.Form["emp_num"]);
                var old_user = db.USERs.FirstOrDefault(o => o.USER_ID == emp_num);
                old_user.USER_STATE = false;

                db.SubmitChanges();

                response.status = ResponseModel.StatusCode.Success;
                response.message = "Delete employee successfully!";
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
