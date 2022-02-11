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

namespace Demo.Controllers;

/// <summary>
/// Контроллер издателей
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PublisherController : ControllerBase
{

    #region Переменные
    internal static int _id = 20;
    private readonly DemoContext _ctx;
    private readonly IHttpClientFactory _httpClientFactory;
    #endregion

    #region Конструктор
    /// <summary>
    /// Конструктор
    /// </summary>
    public PublisherController(DemoContext ctx, IHttpClientFactory httpClientFactory)
    {
        _ctx = ctx;
        _httpClientFactory = httpClientFactory;
    }
    #endregion

    /// <summary>
    /// Получение всех издателей
    /// </summary>        
    [HttpGet("", Name = nameof(GetAllPublishers))]
    public IQueryable<Publisher> GetAllPublishers() =>
        _ctx.Publishers
        .Include(a => a.Authors);

    /// <summary>
    /// Получить издателя по уникальному идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <example>GET api/Authors/1</example>
    [HttpGet("{id}", Name = nameof(GetPublisherById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    [Authorize]
    public ActionResult<Publisher> GetPublisherById(int id)
    {
        var result = _ctx.Publishers.FirstOrDefault(a => a.Id == id);

        if (result == null) return NotFound();

        return result;
    }

    /// <summary>
    /// Получение авторов по уникальному идентификатору издателя
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <example>GET api/Publisher/1/Author</example>
    [HttpGet("{id}/Author", Name = nameof(GetAuthor))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<Author>> GetAuthor(int id)
    {
        var result = _ctx.Publishers.FirstOrDefault(a => a.Id == id);

        if (result == null)
            return BadRequest($"Издательства с заданным Id: {id} не существует");

        return result.Authors.ToArray();
    }

    /// <summary>
    /// Создание нового издательства
    /// </summary>
    /// <param name="publisher">новый издательство</param>
    /// <example>POST api/Authors</example>
    [HttpPost(Name = nameof(CreatePublisher))]
    public void CreatePublisher([FromBody] Publisher publisher)
    {
        publisher.Id = ++_id;
        _ctx.Publishers.Add(publisher);
    }

    /// <summary>
    /// Добавление Автора к издателю
    /// </summary>
    /// <param name="id">Уникальный идентификатор издателя</param>
    /// <param name="authorId">Уникальный идентификатор автора</param>

    /// <example>POST api/Authors</example>
    [HttpPost("{id}/AddAuthor/{authorId}", Name = nameof(AddAuthor))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public ActionResult<bool> AddAuthor(int id, int authorId)
    {
        #region Валидация
        var author = _ctx.Authors.FirstOrDefault(a => a.Id == authorId);
        if (author == null)
            return BadRequest($"Автора с заданным Id: {authorId} не существует");

        var publisher = _ctx.Publishers
            // .Include(p => p.Authors)
            .FirstOrDefault(a => a.Id == id);
        if (publisher == null)
            return BadRequest($"Издателя с заданным Id: {id} не существует");

        var relation = _ctx.PublishersAuthors.FirstOrDefault(a => a.PublisherId == id && a.AuthorId == authorId);
        if (relation != null)
            return BadRequest($"Автор с заданным Id: {authorId} уже добавлен в издалеля {id}");
        #endregion

        //publisher.Authors.Add(author);
        _ctx.PublishersAuthors.Add(new() { AuthorId = authorId, PublisherId = id });
        _ctx.SaveChangesAsync();
        return Ok(true);
    }

    /// <summary>
    /// Изменение автора
    /// </summary>        
    /// <param name="publisher">издатель</param>
    /// <example>PUT api/Publisher</example>
    [HttpPut("", Name = nameof(UpdatePublisher))]
    public ActionResult<Publisher> UpdatePublisher([FromBody] Publisher publisher)
    {
        _ctx.Update(publisher);
        _ctx.SaveChanges();

        return publisher;
    }

    /// <summary>
    /// Удаление издателя
    /// </summary>
    /// <param name="id"></param>
    /// <example>DELETE api/Publisher/1</example>
    [HttpDelete("{id}", Name = nameof(DeletePublisher))]
    public void DeletePublisher(int id)
    {
    }
}
