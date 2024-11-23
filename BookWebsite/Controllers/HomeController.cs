using BookWebsite.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BookWebsite.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string search, string genre, int? page)
        {
            var books = db.Books.AsQueryable();
            var genres = db.Books.Select(p => p.Genre).Distinct().OrderBy(g => g).ToList();

            if (!String.IsNullOrEmpty(search))
            {
                books = books.Where(p => p.Title.Contains(search) ||
                p.Description.Contains(search) ||
                p.Author.Contains(search));
            }
            if (!String.IsNullOrEmpty(genre))
            {
                books = books.Where(b => b.Genre == genre);
            }
            ViewBag.Genres = new SelectList(genres);
            ViewBag.Search = search;
            ViewBag.SelectedGenre = genre;
            int pageSize = 20;
            int pageNumber = page ?? 1;
            return View(books.OrderBy(b => b.Title).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}