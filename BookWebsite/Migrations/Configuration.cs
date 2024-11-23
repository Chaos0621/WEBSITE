namespace BookWebsite.Migrations
{
    using BookWebsite.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BookWebsite.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var random = new Random();

            // Generate a random word
            string GenerateRandomWord()
            {
                const string chars = "abcdefghijklmnopqrstuvwxyz";
                int wordLength = random.Next(3, 10);
                return new string(Enumerable.Repeat(chars, wordLength)
                                            .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            // Generate a random paragraph
            string GenerateParagraph(int wordCount)
            {
                var words = Enumerable.Range(0, wordCount)
                                      .Select(_ => GenerateRandomWord())
                                      .ToArray();
                return $"<p>{string.Join(" ", words)}.</p>";
            }

            // Generate chapter content
            string GenerateChapterContent()
            {
                var paragraphs = Enumerable.Range(0, 10) // 10 paragraphs per chapter
                                           .Select(_ => GenerateParagraph(200)); // 200 words per paragraph
                return string.Join(Environment.NewLine, paragraphs);
            }

            // Define the genre list
            var genres = new[] { "Historical", "Mystery", "Romance", "Science", "Adventure", "Fantasy" };

            // Generate 40 books
            for (int i = 1; i <= 40; i++)
            {
                var bookTitle = $"Book {i}";
                var author = $"Author {random.Next(1, 21)}"; // Random author
                var genre = genres[random.Next(genres.Length)]; // Random genre
                var description = $"This is the description for {bookTitle}.";
                var publishDate = DateTime.Now.AddDays(-random.Next(1, 1000));

                // Check if the book already exists
                var existingBook = context.Books.FirstOrDefault(b => b.Title == bookTitle);
                if (existingBook == null)
                {
                    var book = new Book
                    {
                        Title = bookTitle,
                        Author = author,
                        Genre = genre,
                        Description = description,
                        PublishDate = publishDate
                    };

                    context.Books.Add(book);
                    context.SaveChanges();

                    // Add 40 chapters
                    for (int j = 1; j <= 40; j++)
                    {
                        var chapter = new Chapter
                        {
                            Title = $"Chapter {j}",
                            ChapterNumber = j,
                            Content = GenerateChapterContent(),
                            BookId = book.BookId
                        };
                        context.Chapters.Add(chapter);
                    }

                    // Add 5 comments
                    for (int k = 1; k <= 5; k++)
                    {
                        var comment = new Comment
                        {
                            Content = $"This is comment {k} for {bookTitle}.",
                            PostedDate = DateTime.UtcNow.AddMinutes(-random.Next(1, 5000)), // Random comment time
                            Email = $"user{k}@example.com",
                            BookId = book.BookId
                        };
                        context.Comments.Add(comment);
                    }
                }
            }

            context.SaveChanges();
        }

    }
}
