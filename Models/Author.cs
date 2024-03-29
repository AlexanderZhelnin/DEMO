﻿using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DefaultValueAttribute = HotChocolate.Types.DefaultValueAttribute;

namespace Demo.Models;

/// <summary>
/// Автор
/// </summary>
public class Author
{
    /** Уникальный идентификатор */    
    public int Id { get; set; }

    /** Имя автора */    
    [DefaultValue("Вася")]
    public string Name { get; set; }

    /** Книги автора */
    //[Authorize(Roles = new[] { "admin" })]
    public ICollection<Book> Books { get; set; } = new List<Book>();

    /** Издательства */
    public List<Publisher> Publishers { get; set; } = new List<Publisher>();

    /** Связь многие ко многим Издатели/Авторы */
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [GraphQLIgnore]
    public List<PublishersAuthors> PublishersAuthors { get; set; } = new List<PublishersAuthors>();

    public override string ToString() => $"{Id}:{Name}";
}
