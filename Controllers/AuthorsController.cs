using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Demo.Model;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers
{
    /// <summary>
    /// Контроллер авторов
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        #region Переменные
        internal static int _id = 20;
        private readonly LongPollingQuery<Author> _longpolling;
        private readonly DemoContext _ctx;
        private readonly DemoRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        #endregion

        #region Конструктор
        /// <summary>
        /// Конструктор
        /// </summary>
        public AuthorsController(LongPollingQuery<Author> longpolling, DemoContext ctx, DemoRepository repository, IHttpClientFactory httpClientFactory)
        {
            _longpolling = longpolling;
            _ctx = ctx;
            _repository = repository;
            _httpClientFactory = httpClientFactory;
        }
        #endregion

        /// <summary>
        /// Тестовая функция с получением данных из "другого" микросервиса, с авторизацией
        /// </summary>        
        [HttpGet("TestTrace", Name = nameof(TestTrace))]
        public async Task<IQueryable<Author>> TestTrace()
        {
            var demoClient = new Client.DemoClient("http://localhost:5004", _httpClientFactory.CreateClient("auth"));

            var bs = await demoClient.GetByIdAsync(1);

            //var responce = await _httpClientFactory.CreateClient("auth").GetAsync("http://localhost:5004/api/authors/1");
            //var result = await responce.Content.ReadAsStringAsync();
            return _ctx.Authors;
        }

        /// <summary>
        /// Получение всех авторов
        /// </summary>        
        [HttpGet("", Name = nameof(GetAllAuthors))]
        public IQueryable<Author> GetAllAuthors() =>
            _ctx.Authors
            .Include(a => a.Books)
            .Include(a => a.Publishers);


        /// <summary>
        /// Получить автора по уникальному идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <example>GET api/Authors/1</example>
        [HttpGet("{id}", Name = nameof(GetAuthorById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [Authorize]
        public ActionResult<Author> GetAuthorById(int id)
        {
            var result = _ctx.Authors.FirstOrDefault(a => a.Id == id);

            if (result == null) return NotFound();

            return result;
        }

        /// <summary>
        /// Получение книг по уникальному идентификатору и названию
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        /// <example>GET api/Authors/1/Book/Первая книга Васи</example>
        [HttpGet("{id}/Book/{title?}", Name = nameof(GetBook))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<Book>> GetBook(int id, string title)
        {
            var result = _ctx.Authors.FirstOrDefault(a => a.Id == id);

            if (result == null)
                return BadRequest($"Автора с заданным Id: {id} не существует");

            return title == null
                ? result.Books.ToArray()
                : result.Books.Where(b => string.Equals(b.Title, title, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        /// <summary>
        /// Метод чтения реализующий паттерн LongPolling
        /// </summary>
        /// <param name="marker">маркер после которого происходит чтение данных из очереди</param>
        /// <returns>коллекция результат</returns>
        /// <example>GET api/Authors/GetChanges/</example>
        [HttpGet("getchages/{marker}", Name = nameof(GetChanges))]
        public IAsyncEnumerable<LongPollingValue<Author>> GetChanges(string marker)
        {
            if (marker == "0")
                marker = DateTime.MinValue.ToString("o");

            if (!DateTime.TryParse(marker, out var dt))
                return AsyncEnumerable.Empty<LongPollingValue<Author>>(); ;

            return _longpolling.Read(dt);
        }

        /// <summary>
        /// Создание нового автора
        /// </summary>
        /// <param name="author">новый автор</param>
        /// <example>POST api/Authors</example>
        [HttpPost(Name = nameof(CreateAuthor))]
        public void CreateAuthor([FromBody] Author author)
        {
            author.Id = ++_id;

            _ctx.Authors.Add(author);
            _longpolling.Add(author);
        }

        /// <summary>
        /// Изменение автора
        /// </summary>        
        /// <param name="author"></param>
        /// <example>PUT api/Authors</example>
        [HttpPut("", Name = nameof(UpdateAuthor))]
        public ActionResult<Author> UpdateAuthor([FromBody] Author author)
        {
            //var existAuthor = _ctx.Authors.FirstOrDefault(a => a.Id == author.Id);
            //if (existAuthor == null)
            //    return NotFound($"Не удалось найти автора для обновления {author.Id}");

            //try
            //{
            //    _ctx.Update(author);
            //    _ctx.SaveChanges();
            //}
            //catch (Exception exception)
            //{
            //    //
            //    // тут будет запись ошибки в лог
            //    //

            //    return BadRequest($"Не удалось обновить автора {author.Id}");
            //}

            //return author;

            _repository.Update(author);
            return author;



        }

        /// <summary>
        /// Удаление автора
        /// </summary>
        /// <param name="id"></param>
        /// <example>DELETE api/Authors/1</example>
        [HttpDelete("{id}", Name = nameof(DeleteAuthor))]
        public void DeleteAuthor(int id)
        {
        }
    }
}
