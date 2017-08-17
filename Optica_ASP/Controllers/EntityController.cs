using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Optica_ASP.Models;

namespace Optica_ASP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EntityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Entity
        public ActionResult Index()
        {
            return View(db.Entity.ToList());
        }

        // GET: Entity/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = db.Entity.Find(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        // GET: Entity/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Entity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Direccion,Codigo")] Entity entity)
        {
            if (ModelState.IsValid)
            {
                db.Entity.Add(entity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(entity);
        }

        // GET: Entity/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = db.Entity.Find(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        // POST: Entity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Direccion,Codigo")] Entity entity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entity);
        }

        // GET: Entity/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = db.Entity.Find(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        // POST: Entity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Entity entity = db.Entity.Find(id);
            db.Entity.Remove(entity);
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
    }
}
