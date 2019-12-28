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

    public class AdminController : Controller
    {
        JailEntities db = new JailEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetOfficersData()
        {
            return View(db.officers);
        }

        public ActionResult GetOfficerDetails(int id)
        {

            officer officer = (from item in db.officers
                               where item.officerID == id
                               select item).FirstOrDefault();


            officerStayIn room = (from item in db.officerStayIns
                                  where item.officerid == id
                                  select item).FirstOrDefault();

            OfficerWorkA work = (from item in db.OfficerWorkAs
                                 where item.officerid == id
                                 select item).FirstOrDefault();

            OfficerDetails officerDetails = new OfficerDetails();
            officerDetails.officer = officer;
            if(room!=null)
               officerDetails.room = room.officerRoom;
            if (work != null)
               officerDetails.job = work.Job;

            return View(officerDetails);
        }

        [HttpGet]
        public ActionResult AddOfficer() {

            return View();
        }

        [HttpPost]
        public ActionResult AddOfficer(officer officer, HttpPostedFileBase postedFile)
        {

            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                postedFile.SaveAs(path +officer.citizinID.ToString() + ".jpg");
                officer.imageURL = officer.citizinID.ToString() + ".jpg";
                ViewBag.Message = "File uploaded successfully.";
            }
            officer.AddedDate = DateTime.Now.Date;

            db.Entry(officer).State = EntityState.Added;
            db.SaveChanges();

            Session["officerCitizenID"] = officer.citizinID;

            return Redirect("OfficerAssignment");
        }

        [HttpGet]
        public ActionResult OfficerAssignment(string citizenID) {

            Session["officerCitizenID"] = citizenID;

            var query = from item in db.Jobs
                        where item.workerType == "officer"
                        && item.availablePositionsCount >= 1
                        select item;


            return View(query.ToList<Job>());
        }

        public ActionResult OfficerAssignmentPost(int id)
        {
            var job = (from item in db.Jobs
                       where item.jobID == id
                       select item).FirstOrDefault();
            string sitizenID = Session["officerCitizenID"].ToString();

            var officer = (from item in db.officers
                           where item.citizinID == sitizenID
                           select item).FirstOrDefault();

            var query = (from item in db.OfficerWorkAs
                        where item.officerid == officer.officerID
                        select item).FirstOrDefault();

            //updating the job
            if (query != null)
            {
                query.officer = officer;
                query.Job = job;
                query.jobid = job.jobID;
                query.officerid = officer.officerID;
                query.since = DateTime.Now.Date;
                db.Entry(query).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                //creating new job-officer relation
                OfficerWorkA relation = new OfficerWorkA();

                relation.officer = officer;
                relation.Job = job;
                relation.jobid = job.jobID;
                relation.officerid = officer.officerID;
                relation.since = DateTime.Now.Date;

                db.Entry(relation).State = EntityState.Added;
                db.SaveChanges();

                job.availablePositionsCount--;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();

                AssignARoom(officer);
            }
            return Redirect("~/Admin/GetOfficersData");
        }

        //assign aroom randomly for a specific officer 
        private void AssignARoom(officer officer) {

            var maxRoomID = (from item in db.officerRooms
                        where item.isActive == true
                        orderby item.officerRoomID descending
                        select item.officerRoomID).FirstOrDefault();

            var minRoomID = (from item in db.officerRooms
                        where item.isActive == true
                        orderby item.officerRoomID ascending
                        select item.officerRoomID).FirstOrDefault();

            Random rnd = new Random();
            int selectedRoomID = rnd.Next(minRoomID, maxRoomID);


            var selectedRoom = (from item in db.officerRooms
                      where item.officerRoomID == selectedRoomID
                      select item).FirstOrDefault();

            if (selectedRoom != null) {

                officerStayIn relation = new officerStayIn();
                relation.officer = officer;
                relation.officerid = officer.officerID;
                relation.officerRoom = selectedRoom;
                relation.officerRoomid = selectedRoom.officerRoomID;
                relation.since = DateTime.Now;

                db.Entry(relation).State = EntityState.Added;
                db.SaveChanges();

                selectedRoom.isActive = false;
                db.Entry(selectedRoom).State = EntityState.Modified;
                db.SaveChanges();

            }



        }


        public ActionResult DeleteOfficer(int id) {

            //getting the officer oblect and  deleting  it
            var officer = (from item in db.officers
                           where item.officerID == id
                           select item).FirstOrDefault();
            if (officer != null)
            {
                db.Entry(officer).State = EntityState.Deleted;
            }

            //getting the room relation if exist and deleting it
            var roomRelation = (from item in db.officerStayIns
                                where item.officerid == id
                                select item).FirstOrDefault();

            if (roomRelation != null) {
                db.Entry(roomRelation).State = EntityState.Deleted;
            }

            ////getting the work relation if exist and deleting it
            var WorkRelation = (from item in db.OfficerWorkAs
                                where item.officerid == id
                                select item).FirstOrDefault();

            if (WorkRelation != null) {
                db.Entry(WorkRelation).State = EntityState.Deleted;
            }

            //updaing job position count if the deleted officer had a job
            var job = (from item in db.Jobs
                                where item.jobID == WorkRelation.jobid
                                select item).FirstOrDefault();
            if (job != null)
            {
                job.availablePositionsCount++;
                db.Entry(job).State = EntityState.Modified;
            }


            //updating the room status and set in it active
            var room = (from item in db.officerRooms
                                where item.officerRoomID == id
                                select item).FirstOrDefault();
            if (room != null)
            {
                room.isActive = true;
                db.Entry(room).State = EntityState.Modified;
            }
 
            db.SaveChanges();

            return Redirect("~/Admin/GetOfficersData");
        }

        [HttpGet]
        public ActionResult updateOfficer(int id) {

            var officer = (from item in db.officers
                           where item.officerID == id
                           select item).FirstOrDefault();

            return View(officer);
        }

        [HttpPost]
        public ActionResult updateOfficer(officer officer , HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                postedFile.SaveAs(path + officer.citizinID.ToString() + ".jpg");
                officer.imageURL = officer.citizinID.ToString() + ".jpg";
                ViewBag.Message = "File uploaded successfully.";
            }

            db.Entry(officer).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("~/Admin/GetOfficersData");
        }


    }
}