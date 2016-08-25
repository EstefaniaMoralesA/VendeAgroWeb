﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VendeAgroWeb.Models;

namespace VendeAgroWeb.Controllers.Administrador
{
    public class AdministradorController : Controller
    {
        private VendeAgroEntities db = new VendeAgroEntities();

        // GET: Usuario_Administrador
        public async Task<ActionResult> Index()
        {
            return View(await db.Usuario_Administrador.ToListAsync());
        }

        public async Task<ActionResult> Login()
        {
            return View();
        }

        // GET: Usuario_Administrador/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario_Administrador usuario_Administrador = await db.Usuario_Administrador.FindAsync(id);
            if (usuario_Administrador == null)
            {
                return HttpNotFound();
            }
            return View(usuario_Administrador);
        }

        // GET: Usuario_Administrador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario_Administrador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,nombre,email,token,confirmaEmail,password,activo")] Usuario_Administrador usuario_Administrador)
        {
            if (ModelState.IsValid)
            {
                db.Usuario_Administrador.Add(usuario_Administrador);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(usuario_Administrador);
        }

        // GET: Usuario_Administrador/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario_Administrador usuario_Administrador = await db.Usuario_Administrador.FindAsync(id);
            if (usuario_Administrador == null)
            {
                return HttpNotFound();
            }
            return View(usuario_Administrador);
        }

        // POST: Usuario_Administrador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,nombre,email,token,confirmaEmail,password,activo")] Usuario_Administrador usuario_Administrador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario_Administrador).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(usuario_Administrador);
        }

        // GET: Usuario_Administrador/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario_Administrador usuario_Administrador = await db.Usuario_Administrador.FindAsync(id);
            if (usuario_Administrador == null)
            {
                return HttpNotFound();
            }
            return View(usuario_Administrador);
        }

        // POST: Usuario_Administrador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Usuario_Administrador usuario_Administrador = await db.Usuario_Administrador.FindAsync(id);
            db.Usuario_Administrador.Remove(usuario_Administrador);
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
