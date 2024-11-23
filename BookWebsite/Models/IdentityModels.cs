using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookWebsite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<UserBooks> UserBooks { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Chapters)
                .WithRequired(c => c.Book)
                .HasForeignKey(c => c.BookId)
                .WillCascadeOnDelete(true); 

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Comments)
                .WithRequired(c => c.Book)
                .HasForeignKey(c => c.BookId)
                .WillCascadeOnDelete(true); 

            modelBuilder.Entity<UserBooks>()
                .HasRequired(ub => ub.Book)
                .WithMany(b => b.UserBooks)
                .HasForeignKey(ub => ub.BookId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Chapter>()
                .Property(c => c.BookId)
                .HasColumnAnnotation("Index", 
                new IndexAnnotation(
                    new IndexAttribute("IX_Book_ChapterNumber", 1)
                    { IsUnique = true 
                    }));

            modelBuilder.Entity<Chapter>()
                .Property(c => c.ChapterNumber)
                .HasColumnAnnotation("Index", 
                new IndexAnnotation(
                    new IndexAttribute("IX_Book_ChapterNumber", 2)
                    { IsUnique = true }));

        }
    }
}