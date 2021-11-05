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

namespace Demo.GraphQl
{
    //[ExtendObjectType(typeof(Query))]
    //public class QueryExP
    //{
    //    public IEnumerable<int> GetInts() => new List<int> { 1, 5 };

    //}

    //public class ff
    //{
    //    public IQueryable<Author> Authors([Service] DemoContext ctx) => ctx.Authors;
    //}

    /// <summary>
    /// Запросы GraphQl
    /// Главное отличик запроса в том что его подзапросы выполняются параллельно
    /// </summary>
    public class Query
    {
        private static Dictionary<string, int[]> Permissions = new Dictionary<string, int[]>
        {
            { "admin", new int[] { 1 } },
            { "Вася", new int[] { 2 } }
        };

        //public ff FF() => new ff();

        #region Authors
        /// <summary>
        /// Запрос чтения
        /// </summary>
        /// <param name="ctx">Контекст базы данных Entity</param>
        /// <param name="claimsPrincipal"></param>        
        /// <returns>Авторы</returns>
        [UseProjection]
        [UseFiltering()]
        [UseSorting()]
        public IQueryable<Author> Authors([Service] DemoContext ctx, ClaimsPrincipal claimsPrincipal)
        {
            //var roles = claimsPrincipal.FindAll(ClaimTypes.Role).ToArray();
            //var accessIds = new List<int>();
            //foreach (var r in roles)
            //    if (Permissions.TryGetValue(r.Value, out var ids)) accessIds.AddRange(ids);
            //return ((IQueryable<Author>)ctx.Authors).Where(a => accessIds.Contains(a.Id));

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
        //[Authorize]
        public bool AuthorizeQuery([Service] IHttpContextAccessor context, ClaimsPrincipal claimsPrincipal)
        {
            var user = context.HttpContext.User;
            var username = user.FindFirstValue("preferred_username");
            return true;

        }
        #endregion

    }
}
