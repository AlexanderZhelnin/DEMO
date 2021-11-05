using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Model
{
    /// <summary>
    /// Автор
    /// </summary>
    public class Author
    {
        public Author()
        {

        }

        /** Уникальный идентификатор */
        public int Id { get; set; }

        /** Имя автора */
        [DefaultValue("Вася")]
        public string Name { get; set; }


        /** Книги автора */
        //[Authorize(Roles = new[] { "admin" })]
        public ICollection<Book> Books { get; set; } = new List<Book>();

        /** Издательства */
        public IEnumerable<Publisher> Publishers { get => PublishersAuthors.Select(s => s.Publisher); set { } }

        /** Связь многие ко многим Издатели/Авторы */
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [GraphQLIgnore]
        public List<PublishersAuthors> PublishersAuthors { get; set; } = new List<PublishersAuthors>();
    }
}
