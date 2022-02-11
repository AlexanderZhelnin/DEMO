using System;

namespace Demo.Model;

/** Статус книги */
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
    /** Русский */
    ru = 0,
    /** Английский */
    en = 1,
}

/** Жанр книги */
public enum GenreEnum
{
    /** Фантастика */
    fantastic,
    /** Фантази */
    fantasy,
    /** litRPG */
    litrpg,
    /** Научная литература */
    scientific,
    /** Историческая литература */
    historical
}


/**  Дополнительная информация по книге */
public class BookDetails
{

    /** Уникальный идентификатор */
    public int BookId { get; set; }
    /** Книга */
    public Book Book { get; set; }

    /** Ранк книги */
    public double Rank { get; set; }
    /** Статус книги */
    public StatusEnum Status { get; set; } = StatusEnum.complite;

    /** Стоимость книги в мягком переплёте */
    public decimal PeperbackCost { get; set; }

    /** Стоимость книги в твёрдом переплёте */
    public decimal HardcoverCost { get; set; }
    /** Дата выпуска */
    public DateTime Year { get; set; }

    /** Редакция */
    public string Editor { get; set; }

    /** иллюстратор */
    public string Illustrator { get; set; }

    /** Колличество страниц */
    public int PageCount { get; set; }

    /** Язык */
    public LanguageEnum Language { get; set; }

    /** Возростные ограничения */
    public byte ReadingAge { get; set; }

    /** Жанр */
    public GenreEnum Genre { get; set; }
    /** Отзывы */
    public int Reviews { get; set; }

}
