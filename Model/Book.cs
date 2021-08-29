using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Model
{
    /** Книга */
    public class Book
    {
        /// <summary>
        /// Статус книги
        /// </summary>
        public enum StatusEnum
        {
            /** Книга завершена */
            complite = 0,
            /** Книга пишется */
            write = 1,
        }

        /** Язык */
        public enum LanguageEnum
        {
            ru = 0,
            en = 1,
        }

        /** Жанр книги */
        public enum GenreEnum
        {
            fantastic,
            fantasy,
            litrpg,
            scientific,
            historical
        }

        /** Уникальный идентификатор */
        public int Id { get; set; }

        /** Название книги */
        public string Title { get; set; }

        /** Описание книги */
        public string Description { get; set; }

        /** Дата выпуска */
        public DateTime Year { get; set; }

        /** Уникльный идентификатор автора */
        public int AuthorId { get; set; }

        /** Статус книги */
        public StatusEnum Status { get; set; } = StatusEnum.complite;

        /** Ранк книги */
        public double Rank { get; set; }

        /** Стоимость книги в мягком переплёте */
        public decimal PeperbackCost { get; set; }

        /** Стоимость книги в твёрдом переплёте */
        public decimal HardcoverCost { get; set; }

        /** Издатель */
        public int PublisherId { get; set; }

        /** Редакция */
        public string Editor { get; set; }

        /** иллюстратор */
        public string Illustrator { get; set; }

        /** Колличество страниц */
        public int PageCount { get; set; }

        /** Язык */
        public LanguageEnum Language { get; set; }

        /** Язык */
        public byte ReadingAge { get; set; }

        /** Жанр */
        public GenreEnum Genre { get; set; }


        public string ISBN_10 { get; set; }
        public string ISBN_13 { get; set; }

        /** Отзывы */
        public int Reviews { get; set; }


    }
}
