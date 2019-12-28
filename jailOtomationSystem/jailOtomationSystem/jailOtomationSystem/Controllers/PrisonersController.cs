using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jailOtomationSystem.Models;
using jailOtomationSystem.viewModels;

namespace jailOtomationSystem.Controllers
{
    public class PrisonersController : Controller
    {
        JailEntities db = new JailEntities();
        // GET: Prisoners
       
        public ActionResult GetPrisonersData()
        {
            return View(db.Prisoners);
        }

        public ActionResult GetPrisonerDetails(int id)
        {

            Prisoner prisoner = (from item in db.Prisoners
                                 where item.prisonerID == id
                                 select item).FirstOrDefault();

            Adjudication adj = (from item in db.Prisoners
                                where item.adjudicationid == id
                                select item.Adjudication).FirstOrDefault();

            prisonerSatyIn cell = (from item in db.prisonerSatyIns
                                   where item.prisonerid == id
                                   select item).FirstOrDefault();

            PrisonerWorkA work = (from item in db.PrisonerWorkAs
                                  where item.prisonerid == id
                                  select item).FirstOrDefault();

            prisonerDetails prisonerDetails = new prisonerDetails();
            prisonerDetails.prisoner = prisoner;
            prisonerDetails.cell = cell.prisonerCell;
            if (work.Job != null)
                prisonerDetails.job = work.Job;

            prisonerDetails.adjudication = adj;

            return View(prisonerDetails);
        }

        [HttpGet]
        public ActionResult Add() {


            return View();
        }

        [HttpPost]
        public ActionResult Add(Prisoner prisoner,HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                postedFile.SaveAs(path + prisoner.citizinid.ToString() + ".jpg");
                prisoner.imageURL = prisoner.citizinid.ToString() + ".jpg";
                ViewBag.Message = "File uploaded successfully.";
            }

            db.Entry(prisoner).State = EntityState.Added;

            AssignACell(prisoner);

            return Redirect("~/Prisoners/GetPrisonersData");
        }

        private void AssignACell(Prisoner prisoner)
        {

            var maxCellID = (from item in db.prisonerCells
                             where item.availableCount >= 1
                             orderby item.availableCount descending
                             select item.prisonerCellID).FirstOrDefault();

            var minCellID = (from item in db.prisonerCells
                             where item.availableCount >= 1
                             orderby item.availableCount ascending
                             select item.prisonerCellID).FirstOrDefault();

            Random rnd = new Random();
            int selectedCellID = rnd.Next(minCellID, maxCellID);


            var selectedCell = (from item in db.prisonerCells
                                where item.prisonerCellID == selectedCellID
                                select item).FirstOrDefault();

            if (selectedCell != null)
            {

                prisonerSatyIn relation = new prisonerSatyIn();
                relation.Prisoner = prisoner ;
                relation.prisonerid = prisoner.prisonerID;
                relation.prisonerCell = selectedCell;
                relation.prisonerCellid = selectedCell.prisonerCellID;
                relation.since = DateTime.Now;

                db.Entry(relation).State = EntityState.Added;
                db.SaveChanges();

                selectedCell.availableCount--;
                db.Entry(selectedCell).State = EntityState.Modified;
                db.SaveChanges();

            }

        }

        [HttpGet]
        public ActionResult AssignAJob(int id)
        {
            Session["prisonerID"] = id;

            var query = from item in db.Jobs
                        where item.workerType == "prisoner"
                        && item.availablePositionsCount >= 1
                        select item;

            return View(query.ToList<Job>());
        }

        public ActionResult AssignAjobPost(int id)
        {
            var job = (from item in db.Jobs
                       where item.jobID == id
                       select item).FirstOrDefault();
            int prisonerID =Convert.ToInt32(Session["prisonerID"]);

            var prisoner = (from item in db.Prisoners
                           where item.prisonerID == prisonerID
                           select item).FirstOrDefault();

            var query = (from item in db.PrisonerWorkAs
                         where item.prisonerid == prisoner.prisonerID
                         select item).FirstOrDefault();

            //updating the job
            if (query != null)
            {
                query.Prisoner = prisoner;
                query.Job = job;
                query.jobid = job.jobID;
                query.prisonerid = prisoner.prisonerID;
                query.since = DateTime.Now.Date;

                db.Entry(query).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                //creating new job-prisoner relation
                PrisonerWorkA relation = new PrisonerWorkA();

                relation.Prisoner = prisoner;
                relation.Job = job;
                relation.jobid = job.jobID;
                relation.prisonerid = prisoner.prisonerID;
                relation.since = DateTime.Now.Date;

                db.Entry(relation).State = EntityState.Added;
                db.SaveChanges();

                job.availablePositionsCount--;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Redirect("~/Prisoners/GetPrisonersData");
        }

        public ActionResult DeletePrisoner(int id)
        {

            //getting the prisoner oblect and  deleting  it
            var prisoner = (from item in db.Prisoners
                           where item.prisonerID == id
                           select item).FirstOrDefault();
            if (prisoner != null)
            {
                db.Entry(prisoner).State = EntityState.Deleted;
            }

            //getting the room relation if exist and deleting it
            var roomRelation = (from item in db.prisonerSatyIns
                                where item.prisonerid == id
                                select item).FirstOrDefault();

            if (roomRelation != null)
            {
                db.Entry(roomRelation).State = EntityState.Deleted;
            }

            ////getting the work relation if exist and deleting it
            var WorkRelation = (from item in db.PrisonerWorkAs
                                where item.prisonerid == id
                                select item).FirstOrDefault();

            if (WorkRelation != null)
            {
                db.Entry(WorkRelation).State = EntityState.Deleted;
            }

            //updaing job position count if the deleted prisoner had a job
            var job = (from item in db.Jobs
                       where item.jobID == WorkRelation.jobid
                       select item).FirstOrDefault();
            if (job != null)
            {
                job.availablePositionsCount++;
                db.Entry(job).State = EntityState.Modified;
            }


            //updating the room status and set in it active
            var room = (from item in db.prisonerCells
                        where item.prisonerCellID == id
                        select item).FirstOrDefault();
            if (room != null)
            {
                room.availableCount++;
                db.Entry(room).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Redirect("~/Admin/GetprisonersData");
        }

        [HttpGet]
        public ActionResult updateprisoner(int id)
        {

            var prisoner = (from item in db.Prisoners
                           where item.prisonerID == id
                           select item).FirstOrDefault();

            return View(prisoner);
        }

        [HttpPost]
        public ActionResult updateprisoner(Prisoner prisoner, HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                postedFile.SaveAs(path + prisoner.citizinid.ToString() + ".jpg");
                prisoner.imageURL = prisoner.citizinid.ToString() + ".jpg";
                ViewBag.Message = "File uploaded successfully.";
            }

            db.Entry(prisoner).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("~/Admin/GetprisonersData");
        }
    }
}