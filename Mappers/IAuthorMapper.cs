using System;
using System.Linq.Expressions;
using Demo.Models;
using Mapster;

namespace Demo;

/// <summary>
/// Интерфейс для генерации сопоставления
/// </summary>
[Mapper]
public interface IAuthorMapper
{
    Expression<Func<Author, AuthorDTO>> AuthorProjection { get; }
    AuthorDTO MapTo(Author author);
    AuthorDTO MapTo(Author author, AuthorDTO authordto);

    //Author MapTo(AuthorDTO author);
    //Author MapTo(AuthorDTO authordto, Author author);

}
