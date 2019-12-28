using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jailOtomationSystem.Models;
using jailOtomationSystem.viewModels;

namespace jailOtomationSystem.Controllers
{
    public class VisitsController : Controller
    {
        JailEntities db = new JailEntities();

        // GET: Visits
        public ActionResult Index()
        {
            return View(db.visits);
        }

        // GET: Visits/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Visits/Edit/5
        public ActionResult Edit(int id)
        {
            var visit = (from item in db.visits
                         where item.visitorID == id
                         select item).FirstOrDefault();

            return View(visit);
        }

        // POST: Visits/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, visit visit)
        {
            try
            {
                // TODO: Add update logic here
                db.Entry(visit).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Visits/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Visits/Delete/5
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
