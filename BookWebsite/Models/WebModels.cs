using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookWebsite.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [MaxLength(200, ErrorMessage = "Author name cannot exceed 200 characters.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [DataType(DataType.MultilineText)] // Multiline text format
        public string Description { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Publish Date is required.")]
        [DataType(DataType.Date)] // Calendar date picker in UI
        public DateTime PublishDate { get; set; }

        // Navigation property
        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<UserBooks> UserBooks { get; set; } = new List<UserBooks>();
    }


    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(320, ErrorMessage = "Email cannot exceed 320 characters.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        // Foreign key
        public int BookId { get; set; }

        // Navigation property
        [ForeignKey("BookId")]
        public Book Book { get; set; }
    }


    public class Chapter
    {
        [Key]
        public int ChapterId { get; set; }

        [Required(ErrorMessage = "Chapter Title is required.")]
        [MaxLength(200, ErrorMessage = "Chapter Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Chapter Number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Chapter Number must be greater than 0.")]
        public int ChapterNumber { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        // Foreign key
        public int BookId { get; set; }

        // Navigation property
        [ForeignKey("BookId")]
        public Book Book { get; set; }
    }

    public class UserBooks
    {
        [Key]
        public int UserBookId { get; set; }

        [Required]
        [MaxLength(320)]
        public string Email { get; set; }

        // Foreign key
        public int BookId { get; set; }

        // Navigation property
        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required]
        public bool IsFavorite { get; set; } = false;

        [Required]
        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    }
}