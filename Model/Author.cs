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
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя автора
        /// </summary>        
        [DefaultValue("Вася")]
        public string Name { get; set; }

        /// <summary>
        /// Книги автора
        /// </summary>
        [Authorize(Roles = new[] { "admin" })]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
