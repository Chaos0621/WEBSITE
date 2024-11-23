using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using BookWebsite.Models;
using PagedList;

namespace BookWebsite.Controllers
{
    public class ChaptersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Chapters
        public ActionResult Index(string title, string sortOrder, int page = 1)
        {
            int PageSize = 10;
            var books = db.Books.Select(b => b.Title).Distinct().OrderBy(b => b).ToList();
            if (string.IsNullOrEmpty(title) && books.Any())
            {
                title = books.First();
            }
            ViewBag.ChapterNumberSortParam = String.IsNullOrEmpty(sortOrder) ? "chapter_num_desc" : "";
            ViewBag.books = new SelectList(books, title);
            ViewBag.SelectedBooks = title;
            var chapters = db.Chapters.Include(c => c.Book);
            if (!string.IsNullOrEmpty(title))
            {
                chapters = chapters.Where(c => c.Book.Title == title);
            }
            switch (sortOrder)
            {
                case "chapter_num_desc":
                    chapters = chapters.OrderByDescending(c => c.ChapterNumber);
                    break;
                default:
                    chapters = chapters.OrderBy(c => c.ChapterNumber);
                    break;
            }
            var pagedChapters = chapters.ToPagedList(page, PageSize);
            return View(pagedChapters);
        }

        // GET: Chapters/Details/5
        public ActionResult Details(int? id)
        {
            var chapter = db.Chapters.Include(c => c.Book).FirstOrDefault(c => c.ChapterId == id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (chapter == null)
            {
                return HttpNotFound();
            }
            // get previous and after
            var previousChapter = db.Chapters
                .Where(c => c.BookId == chapter.BookId && c.ChapterNumber < chapter.ChapterNumber)
                .OrderByDescending(c => c.ChapterNumber)
                .FirstOrDefault();

            var nextChapter = db.Chapters
                .Where(c => c.BookId == chapter.BookId && c.ChapterNumber > chapter.ChapterNumber)
                .OrderBy(c => c.ChapterNumber)
                .FirstOrDefault();
            // pass to view
            ViewBag.PreviousChapterId = previousChapter?.ChapterId;
            ViewBag.NextChapterId = nextChapter?.ChapterId;

            return View(chapter);
        }

        // GET: Chapters/Create
        public ActionResult Create(int bookId)
        {
            ViewBag.BookId = bookId;
            return View();
        }

        // POST: Chapters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,ChapterNumber,Content,BookId")] Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                bool chapterExists = db.Chapters.Any(c => c.BookId == chapter.BookId && c.ChapterNumber == chapter.ChapterNumber);
                if (chapterExists)
                {
                    ModelState.AddModelError("ChapterNumber", "The chapter number must be unique for this book.");
                    ViewBag.BookId = chapter.BookId;
                    return View(chapter);
                }

                if (!string.IsNullOrWhiteSpace(chapter.Content))
                {
                    var paragraphs = chapter.Content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    chapter.Content = HttpUtility.HtmlEncode(chapter.Content);
                }


                db.Chapters.Add(chapter);
                db.SaveChanges();
                return RedirectToAction("Details", "Books", new { id = chapter.BookId });
            }

            ViewBag.BookId = chapter.BookId;
            return View(chapter);
        }

        // GET: Chapters/Edit/5
        public ActionResult Edit(int? id)
        {
            var chapter = db.Chapters.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (chapter == null)
            {
                return HttpNotFound();
            }
            chapter.Content = RemoveHtmlTags(chapter.Content);
            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title", chapter.BookId);
            return View(chapter);
        }

        private string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
        }

        // POST: Chapters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChapterId,Title,ChapterNumber,Content,BookId")] Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(chapter.Content))
                {
                    var paragraphs = chapter.Content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    chapter.Content = string.Join("", paragraphs.Select(p => $"<p>{p.Trim()}</p>"));
                }

                db.Entry(chapter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = chapter.ChapterId });
            }
            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title", chapter.BookId);
            return View(chapter);
        }

        // GET: Chapters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            db.Chapters.Remove(chapter);
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
