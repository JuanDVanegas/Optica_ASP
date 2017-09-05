using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Optica_ASP.Models;

namespace Optica_ASP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EntityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Entity
        public async Task<ActionResult> Index()
        {
            return View(await db.Entity.ToListAsync());
        }

        // GET: Entity/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = await db.Entity.FindAsync(id);
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
        public async Task<ActionResult> Create([Bind(Include = "EntityId,Nombre,Direccion,Codigo")] Entity entity)
        {
            if (ModelState.IsValid)
            {
                db.Entity.Add(entity);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(entity);
        }

        // GET: Entity/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = await db.Entity.FindAsync(id);
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
        public async Task<ActionResult> Edit([Bind(Include = "EntityId,Nombre,Direccion,Codigo")] Entity entity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entity).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(entity);
        }

        // GET: Entity/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = await db.Entity.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        // POST: Entity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Entity entity = await db.Entity.FindAsync(id);
            db.Entity.Remove(entity);
            await db.SaveChangesAsync();
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
