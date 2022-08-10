using System.Collections.Generic;

namespace Demo.Models;

/// <summary>
/// Издатель
/// </summary>
public class Publisher
{
    public Publisher()
    {

    }
    /** Уникальный идентификатор */
    public int Id { get; set; }

    /** Название издателя */
    public string Name { get; set; }

    /** Авторы */
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<Author> Authors { get; set; } = new List<Author>();

    /** Связь многие ко многим Издатели/Авторы */
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<PublishersAuthors> PublishersAuthors { get; set; } = new List<PublishersAuthors>();

}
