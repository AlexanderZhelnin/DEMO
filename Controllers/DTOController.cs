using Demo.DB;
using Demo.Models;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers;

/** Пример контроллера для показа работы Mapster */
[Route("api/[controller]")]
[ApiController]
public class DTOController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly DemoContext _ctx;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="mapper">мапинг</param>
    /// <param name="ctx">контекст базы данных</param>
    public DTOController(
        IMapper mapper,
        DemoContext ctx
        )
    {
        _mapper = mapper;
        _ctx = ctx;
    }

    /// <summary>
    /// Получить авторов DTO
    /// </summary>
    /// <returns></returns>
    [HttpGet("", Name = nameof(GetDTOAuthors))]
    public IQueryable<AuthorDTO> GetDTOAuthors()
    {                
        return _mapper
            .From(_ctx.Authors)
            .ProjectToType<AuthorDTO>();
    }


    /// <summary>
    /// Получить автора по уникальному идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <example>GET api/Authors/1</example>
    [HttpGet("{id}", Name = nameof(GetAuthorDTOById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]    
    public ActionResult<AuthorDTO> GetAuthorDTOById(int id)
    {
        var result = _ctx.Authors.FirstOrDefault(a => a.Id == id);

        if (result == null) return NotFound();
                
        return result.Adapt<AuthorDTO>();
    }
}
