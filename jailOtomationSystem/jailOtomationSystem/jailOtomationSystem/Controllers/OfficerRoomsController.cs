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
    public class OfficerRoomsController : Controller
    {

        JailEntities db = new JailEntities();
        // GET: OfficerRooms
        public ActionResult Index()
        {
            return View(db.officerRooms);
        }

        // GET: OfficerRooms/Details/5
        public ActionResult Details(int id)
        {
           

            var relation = (from item in db.officerStayIns
                            where item.officerRoomid == id
                            select item).FirstOrDefault();

            if (relation != null)
            {
                return View(relation);
            }
            else
            {
                var officerRoom = (from item in db.officerRooms
                                   where item.officerRoomID == id
                                   select item).FirstOrDefault();

                officerStayIn tempRelation = new officerStayIn();

                tempRelation.officerRoom = officerRoom;
                tempRelation.officerRoomid = officerRoom.officerRoomID;

                return View(tempRelation);

            }
            
        }

        // GET: OfficerRooms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OfficerRooms/Create
        [HttpPost]
        public ActionResult Create(officerRoom room)
        {
            try
            {
                // TODO: Add insert logic here
                room.isActive = true;
                db.Entry(room).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: OfficerRooms/Edit/5
        public ActionResult Edit(int id)
        {
          var  room = (from item in db.officerRooms
                    where item.officerRoomID == id
                    select item).FirstOrDefault();

            return View(room);
        }

        // POST: OfficerRooms/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, officerRoom room)
        {
            try
            {
                // TODO: Add update logic here
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
    }
}
