using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookWebsite.Models;
using BookWebsite.ViewModels;
using PagedList;

namespace BookWebsite.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        public ActionResult Index(string search, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var books = db.Books.AsQueryable();
            if (!String.IsNullOrEmpty(search))
            {
                books = books.Where(p => p.Title.Contains(search) ||
                p.Description.Contains(search) ||
                p.Author.Contains(search));
            }
            var pagedBooks = books.OrderBy(b => b.Title).ToPagedList(pageNumber, pageSize);

            ViewBag.Search = search;
            return View(pagedBooks);
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id, string sortBy, int chapterPage = 1, int commentPage = 1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var book = db.Books
                .Include(b => b.Chapters)
                .Include(b => b.Comments)
                .FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return HttpNotFound();
            }

            // Sort chapters
            var sortedChapters = !string.IsNullOrEmpty(sortBy) && sortBy == "desc"
                ? book.Chapters.OrderByDescending(c => c.ChapterNumber).ToList()
                : book.Chapters.OrderBy(c => c.ChapterNumber).ToList();

            // Paginate
            int pageSize = 10;
            var pagedChapters = sortedChapters.ToPagedList(chapterPage, pageSize);
            var pagedComments = book.Comments
                .OrderByDescending(c => c.PostedDate)
                .ToPagedList(commentPage, pageSize);

            // Create ViewModel
            var viewModel = new BookDetailsViewModel
            {
                Book = book,
                PagedChapters = pagedChapters,
                PagedComments = pagedComments,
                CurrentSort = sortBy
            };

            return View(viewModel);
        }


        // GET: Books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookId,Title,Author,Description,Genre,PublishDate")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookId,Title,Author,Description,Genre,PublishDate")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
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
