using Demo.Model;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using HotChocolate.Types.Pagination;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Demo.GraphQl;

/// <summary>
/// Запросы GraphQl
/// Главное отличик запроса в том что его подзапросы выполняются параллельно
/// </summary>
public class Query
{
    private ILogger<Query> _logger;

    //private static Dictionary<string, int[]> Permissions = new Dictionary<string, int[]>
    //{
    //    { "admin", new int[] { 1 } },
    //    { "Вася", new int[] { 2 } }
    //};

    /// <summary>
    /// Конструктор
    /// </summary>
    public Query(ILogger<Query> logger)
    {
        _logger = logger;

        logger.LogInformation("Инициализация");
    }

    /// <summary>
    /// Возвращает первую версию
    /// </summary>
    /// <returns></returns>
    public Api1 V1() =>
        new Api1();


    #region Authors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="after"></param>
    /// <param name="first"></param>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    [UseDbContext(typeof(DemoContext))]
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    public Connection<Author> MyLogicPaginAuthors(string? after, int? first, string sortBy, [ScopedService] DemoContext ctx)
    {

        var pageSize = first ?? 10;
        var authors = ((IQueryable<Author>)ctx.Authors);
        var count = authors.Count();
        var hasNextPage = false;

        if (after != null)
        {
            var split = after.IndexOf('_');
            var id = Convert.ToInt32(after[..split]);
            var name = after[(split + 1)..];

            authors =
                authors
                    .Where(a => string.Compare(a.Name, name) == 1 || (a.Name == name && a.Id > id));
        }

        var edges =
            authors
                    .OrderBy(a => a.Name)
                    .ThenBy(a => a.Id)
                    .Select(a => new Edge<Author>(a, $"{a.Id}_{a.Name}"))
                    .Take(pageSize + 1)
                    .ToList();


        hasNextPage = edges.Count > pageSize;

        edges = edges.SkipLast(1).ToList();

        var pageInfo =
            new ConnectionPageInfo(
                    hasNextPage,
                    false,
                    edges.FirstOrDefault()?.Cursor,
                    edges.LastOrDefault()?.Cursor);

        var connection =
            new Connection<Author>(
                    edges,
                    pageInfo,
                    ct => ValueTask.FromResult(count));

        return connection;
    }

    /// <summary>
    /// Запрос чтения
    /// </summary>
    /// <param name="ctx">Контекст базы данных Entity</param>            
    /// <returns>Авторы</returns>        
    [UseDbContext(typeof(DemoContext))]
    //[UsePaging(IncludeTotalCount = true)]
    //[UseOffsetPaging]
    [UseProjection]
    [UseFiltering()]
    //[UseFiltering(typeof(AuthorFilterType))]
    [UseSorting()]
    public IQueryable<Author> Authors([ScopedService] DemoContext ctx)//, ClaimsPrincipal claimsPrincipal)
    {
        #region MyRegion
        //var roles = claimsPrincipal.FindAll(ClaimTypes.Role).ToArray();
        //var accessIds = new List<int>();
        //foreach (var r in roles)
        //    if (Permissions.TryGetValue(r.Value, out var ids)) accessIds.AddRange(ids);
        //return ((IQueryable<Author>)ctx.Authors).Where(a => accessIds.Contains(a.Id)); 
        #endregion

        //var resutl = ((IQueryable<Author>)ctx.Authors).Where(a => EF.Functions.ILike(a.Name, "автор 4")).ToList();
        var resutl = ((IQueryable<Author>)ctx.Authors.AsNoTracking()).Where(a => a.Name.ToLower() == "автор 4".ToLower()).ToList();

        return ctx.Authors;
    }

    #endregion
    #region AuthorsById           
    /// <summary>
    /// Чтение по уникальным идентификаторам, это функция по факту не нужна, легко заменяется функцией Authors с фильтром
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="ids"></param>
    /// <returns>Авторы</returns>           
    [UseProjection]
    public IQueryable<Author> AuthorsByIds([Service] DemoContext ctx, IEnumerable<int> ids) =>
              ((IQueryable<Author>)ctx.Authors).Where(a => ids.Contains(a.Id));
    #endregion

    #region AuthorById
    /// <summary>
    /// Получить автора по иникальному идентификатору
    /// </summary>
    /// <param name="ctx">Контекст базы данных</param>
    /// <param name="id">Уникальный идентификатор книги</param>
    /// <returns>Автор книги</returns>
    /// <exception cref="ArgumentException"></exception>
    public Author AuthorById([Service] DemoContext ctx, int id)
    {
        var author = ((IQueryable<Author>)ctx.Authors).FirstOrDefault(a => a.Id == id);

        if (author == null) throw new ArgumentException($"Автор с заданным id: {id} не существует");

        return author;
    }
    #endregion

    #region Book           
    /// <summary>
    /// Запрос получения книг
    /// </summary>
    /// <param name="ctx">Контекст базы данных Entity</param>
    /// <returns>Книги</returns>
    [UseProjection]
    [UseFiltering()]
    [UseSorting()]
    public IQueryable<Book> Books([Service] DemoContext ctx) => ctx.Books;
    #endregion

    #region AuthorizeQuer           
    /// <summary>
    /// Тестовая функция с авторизацией
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    [Authorize(Roles = new[] { "admin" })]
    public bool AuthorizeQuery([Service] IHttpContextAccessor context, ClaimsPrincipal claimsPrincipal)
    {
        var user = context.HttpContext.User;
        var username = user.FindFirstValue("preferred_username");
        return true;

    }
    #endregion

}

public class Api1
{
    /// <summary>
    /// Запрос чтения
    /// </summary>
    /// <param name="ctx">Контекст базы данных Entity</param>            
    /// <returns>Авторы</returns>        
    [UseDbContext(typeof(DemoContext))]
    [UseProjection]
    [UseFiltering()]
    [UseSorting()]
    public IQueryable<Author> Authors([ScopedService] DemoContext ctx)
    {
        return ctx.Authors;
    }
}
