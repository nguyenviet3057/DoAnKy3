﻿using System;
using System.Collections.Generic;
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