using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jailOtomationSystem.Models;

namespace jailOtomationSystem.Controllers
{
    public class JobController : Controller
    {
        JailEntities db = new JailEntities();
        // GET: Job
        public ActionResult AvailableJobs()
        {
            return View();
        }

        public ActionResult GetJobs()
        {
            return View(db.Jobs);
        }
        [HttpGet]
        public ActionResult Edit(int id)

        {
            var job = (from item in db.Jobs
                       where item.jobID == id
                       select item).FirstOrDefault();

            return View(job);
        }

        [HttpPost]

        public ActionResult Edit(Job job)
        {
            var facilityID = (from item in db.facilities
                              where item.name == job.facility.name
                              select item.facilityID).FirstOrDefault();

            job.facilityid = facilityID;

            db.Entry(job).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Redirect("~/Job/GetJobs");
        }

        public ActionResult Delete(int id)
        {

            return Redirect("~/Job/GetJobs");
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        //linq(lambda expression)
        public string Add(Job job)
        {
            var facilityID = (from item in db.facilities
                         where item.name == job.facility.name
                         select item.facilityID).FirstOrDefault();

            job.facilityid = facilityID;

            db.Entry(job).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();

            return "job has been added successfully";
        }


  

    }
}