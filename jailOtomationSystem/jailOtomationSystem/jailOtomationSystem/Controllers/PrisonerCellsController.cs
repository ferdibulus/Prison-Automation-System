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
    public class PrisonerCellsController : Controller
    {
        JailEntities db = new JailEntities();

        // GET: PrisonerCells
        public ActionResult Index()
        {
            return View(db.prisonerCells);
        }

        // GET: PrisonerCells/Details/5
        public ActionResult Details(int id)
        {
            //sending a relation object which includes the cell info and the  Resident prisoners
            var relation = (from item in db.prisonerSatyIns
                            where item.prisonerCellid == id
                            select item).ToList<prisonerSatyIn>();

            cellViewModel cellInfo = new cellViewModel();

            if (relation != null)
            {
                cellInfo.cell = relation[0].prisonerCell;
                cellInfo.prisoners = new List<Prisoner>();
                for (int i=0;i<relation.Count;i++)
                {
                    Prisoner prisoner = new Prisoner();
                    prisoner = relation[i].Prisoner;
                    cellInfo.prisoners.Add(prisoner);
                }

                return View(cellInfo);
            }
            else
            {
                var cell = (from item in db.prisonerCells
                            where item.prisonerCellID == id
                            select item).FirstOrDefault();

                cellInfo.cell = cell;

                return View(cellInfo);

            }
        }

        // GET: PrisonerCells/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrisonerCells/Create
        [HttpPost]
        public ActionResult Create(prisonerCell prisonerCell)
        {
            try
            {
                // TODO: Add insert logic here
                prisonerCell.availableCount = prisonerCell.capacity;
                db.Entry(prisonerCell).State = EntityState.Added;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PrisonerCells/Edit/5
        public ActionResult Edit(int id)
        {
            var cell = (from item in db.prisonerCells
                        where item.prisonerCellID == id
                        select item).FirstOrDefault();

            return View(cell);
        }

        // POST: PrisonerCells/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, prisonerCell prisonerCell)
        {
            try
            {
                // TODO: Add update logic here
                db.Entry(prisonerCell).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

        // GET: PrisonerCells/Delete/5

    }
}
