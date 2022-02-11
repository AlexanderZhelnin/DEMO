using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Model;

/** Книга */
public class Book
{
    /** Уникальный идентификатор */
    public int Id { get; set; }

    /** Название книги */
    public string Title { get; set; }

    /** Описание книги */
    public string Description { get; set; }

    /** Обложка */
    public string ImageUrl { get; set; }

    /** Уникльный идентификатор автора */
    public int AuthorId { get; set; }
    /** Автор */
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Author Author { get; set; }

    /** Идентификатор ISBN10 */
    public string ISBN_10 { get; set; }
    /** Идентификатор ISBN13 */
    public string ISBN_13 { get; set; }

    /** Дополнительная информация по книге */
    public BookDetails Details { get; set; }

}
