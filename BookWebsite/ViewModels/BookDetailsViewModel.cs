using BookWebsite.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookWebsite.ViewModels
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; }
        public IPagedList<Chapter> PagedChapters { get; set; }
        public IPagedList<Comment> PagedComments { get; set; }
        public string CurrentSort { get; set; }
    }

}