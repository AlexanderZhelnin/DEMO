using GreenDonut;

namespace Demo.Models;

/// <summary>
/// Автор для передачи данных
/// </summary>
public partial class AuthorDTO
{
    /** Уникальный идентификатор */
    public int Id { get; set; }

    /** Имя автора */
    public string Name { get; set; }

    public string Name1 { get; set; }
}