using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HackathonPMA.Models;
using PagedList;

namespace HackathonPMA.Controllers
{
    [Authorize]
    public class InventoriesController : Controller
    {
        private Entities db = new Entities();

        // GET: Inventories
        public ActionResult Index(string sortBy, string currentFilter, string searchBy, int? page)
        {
            ViewBag.CurrentSort = sortBy;
            ViewBag.NameSort = string.IsNullOrEmpty(sortBy) ? "Name desc" : "";
            ViewBag.QuantitySort = sortBy == "Quantity" ? "Quantity desc" : "Quantity";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";
            ViewBag.PriceSort = sortBy == "Price" ? "Price desc" : "Price";

            if (searchBy != null)
            {
                page = 1;
            }
            else
            {
                searchBy = currentFilter;
            }

            ViewBag.CurrentFilter = searchBy;

            var inventories = from s in db.Inventories
                        select s;
            if (!String.IsNullOrEmpty(searchBy))
            {
                inventories = inventories.Where(s => s.Name.Contains(searchBy)
                                       || s.Description.Contains(searchBy)
                                       || s.Quantity.ToString().Contains(searchBy)
                                       || s.Price.ToString().Contains(searchBy));
            }

            switch (sortBy)
            {
                case "Name desc":
                    inventories = inventories.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    inventories = inventories.OrderBy(s => s.Name);
                    break;
                case "Description desc":
                    inventories = inventories.OrderByDescending(s => s.Description);
                    break;
                case "Description":
                    inventories = inventories.OrderBy(s => s.Description);
                    break;
                case "Price desc":
                    inventories = inventories.OrderByDescending(s => s.Price);
                    break;
                case "Price":
                    inventories = inventories.OrderBy(s => s.Price);
                    break;
                case "Quantity desc":
                    inventories = inventories.OrderByDescending(s => s.Quantity);
                    break;
                case "Quantity":
                    inventories = inventories.OrderBy(s => s.Quantity);
                    break;
                default:
                    inventories = inventories.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(inventories.ToPagedList(pageNumber, pageSize));
           // return View(inventories.ToList());
        }

        // GET: Inventories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // GET: Inventories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Quantity,Price")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                db.Inventories.Add(inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inventory);
        }

        // GET: Inventories/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Quantity,Price")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventory inventory = db.Inventories.Find(id);
            db.Inventories.Remove(inventory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult doesInventoryNameExist(string Name, string oldName)
        {
            if (oldName.Equals("create") || (Name.Trim().ToLower() != oldName.Trim().ToLower()))
            {
                return Json(!db.Inventories.Any(x => x.Name == Name), JsonRequestBehavior.AllowGet);
            }            
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
