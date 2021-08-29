using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace Demo.Model
{
    /** */
    public class DemoContext : DbContext
    {
        /// <summary>
        /// Констрктор DemoContext
        /// </summary>
        /// <param name="o">свойства контекста</param>
        public DemoContext(DbContextOptions o) : base(o)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        }

        /** Авторы */
        public DbSet<Author> Authors { get; set; }
        /** Книги */
        public DbSet<Book> Books { get; set; }
                
        /// <summary>
        /// Настройка свойств модели
        /// </summary>        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var authors = new Author[]
               {
                    new() { Id = 1, Name = "Вася", },
                    new() { Id = 2, Name = "Петя", }
               };

         
            modelBuilder.Entity<Author>(b =>
            {

                b.HasData(authors);
                b.OwnsMany(v => v.Books).HasData(

                    new() { Id = 3, AuthorId = 1, Title = "Первая книга Васи", Year = new DateTime(2000, 1, 1), PublisherId = 1, Description = "Биографическое описание жизни", Genre = Book.GenreEnum.historical, HardcoverCost = 100, Illustrator = "МИКЕЛАНДЖЕЛО", Editor = "Не известен", Language = Book.LanguageEnum.ru, PageCount = 100, ReadingAge = 18, Rank = 7.112 },
                    new() { Id = 4, AuthorId = 1, Title = "Вторая книга Васи", Year = new DateTime(2001, 1, 1), PublisherId = 1, Description = "Фантастическая книга о приключениях", Genre = Book.GenreEnum.fantastic, HardcoverCost = 110, Illustrator = "ЙОХАННЕС ВЕРМЕЕР", Editor = "Не известен", Language = Book.LanguageEnum.ru, PageCount = 200, ReadingAge = 12, Rank = 4.343 },
                    new() { Id = 5, AuthorId = 1, Title = "Третья книга Васи", Year = new DateTime(2002, 1, 1), PublisherId = 1, Description = "Историческая книга", Genre = Book.GenreEnum.historical, HardcoverCost = 120, Illustrator = "ПАБЛО ПИКАССО", Editor = "Не известен", Language = Book.LanguageEnum.ru, PageCount = 300, ReadingAge = 10, Rank = 9.2 },
                    new() { Id = 6, AuthorId = 2, Title = "Первая книга Пети", Year = new DateTime(2003, 1, 1), PublisherId = 2, Description = "Фентази об эльфах", Genre = Book.GenreEnum.fantasy, HardcoverCost = 130, Illustrator = "ВИНСЕНТ ВАН ГОГ", Editor = "Не известен", Language = Book.LanguageEnum.ru, PageCount = 400, ReadingAge = 5, Rank = 8.345 },
                    new() { Id = 7, AuthorId = 2, Title = "Вторая книга Пети", Year = new DateTime(2004, 1, 1), PublisherId = 2, Description = "Научная литература, докозательство 3-й теорему Фихтенгольца", Genre = Book.GenreEnum.scientific, HardcoverCost = 140, Illustrator = "РЕМБРАНДТ ВАН РЕЙН", Editor = "Не известен", Language = Book.LanguageEnum.en, PageCount = 500, ReadingAge = 10, Rank = 1 },
                    new() { Id = 8, AuthorId = 2, Title = "Третья книга Пети", Year = new DateTime(2005, 1, 1), PublisherId = 2, Description = "Научная фантастика и приключение героя в далёком космосе", Genre = Book.GenreEnum.fantastic, HardcoverCost = 150, Illustrator = "ЛЕОНАРДО ДА ВИНЧИ", Editor = "Не известен", Language = Book.LanguageEnum.ru, PageCount = 600, ReadingAge = 18, Rank = 5 }
                    );
            });



        }

    }
}
