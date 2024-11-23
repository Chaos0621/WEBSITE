using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookWebsite.Models;

namespace BookWebsite.Controllers
{
    public class UserBooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserBooks
        public ActionResult Index()
        {
            var userBooks = db.UserBooks.Include(u => u.Book);
            return View(userBooks.ToList());
        }

        // GET: UserBooks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserBooks userBooks = db.UserBooks.Find(id);
            if (userBooks == null)
            {
                return HttpNotFound();
            }
            return View(userBooks);
        }

        // GET: UserBooks/Create
        public ActionResult Create()
        {
            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title");
            return View();
        }

        // POST: UserBooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserBookId,Email,BookId,IsFavorite,LastAccessed")] UserBooks userBooks)
        {
            if (ModelState.IsValid)
            {
                db.UserBooks.Add(userBooks);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title", userBooks.BookId);
            return View(userBooks);
        }

        // GET: UserBooks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserBooks userBooks = db.UserBooks.Find(id);
            if (userBooks == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title", userBooks.BookId);
            return View(userBooks);
        }

        // POST: UserBooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserBookId,Email,BookId,IsFavorite,LastAccessed")] UserBooks userBooks)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userBooks).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title", userBooks.BookId);
            return View(userBooks);
        }

        // GET: UserBooks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserBooks userBooks = db.UserBooks.Find(id);
            if (userBooks == null)
            {
                return HttpNotFound();
            }
            return View(userBooks);
        }

        // POST: UserBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserBooks userBooks = db.UserBooks.Find(id);
            db.UserBooks.Remove(userBooks);
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
