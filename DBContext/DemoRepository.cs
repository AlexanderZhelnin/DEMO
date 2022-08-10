using Demo.Models;
using System;
using System.Linq;

namespace Demo.DB;

/** Демо репозиторий */
public class DemoRepository
{
    private readonly DemoContext _ctx;

    /** Конструктор */
    public DemoRepository(DemoContext ctx)
    {
        _ctx = ctx;
    }

    /// <summary>
    /// Получить автора по иникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор</param>
    /// <returns></returns>
    public Author Get(int id)
    {
        var author = _ctx.Authors.FirstOrDefault(x => x.Id == id);
        if (author == null) throw new ArgumentException($"Автора с заданным Id: {id} не существует");
        return author;
    }
    /// <summary>
    /// Обновить автора
    /// </summary>
    /// <param name="author">автор для обновления</param>
    public void Update(Author author)
    {
        var a = Get(author.Id);

        try
        {
            _ctx.Update(author);
            _ctx.SaveChanges();
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось обновить автора {author.Id}");
        }
    }
}
