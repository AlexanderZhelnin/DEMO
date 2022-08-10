using Demo.Models;
using HotChocolate;
using HotChocolate.Types;

namespace Demo.GraphQl;

/// <summary>
/// Подписки
/// </summary>
public class Subscription
{
    /// <summary>
    /// Добавлен новый автор
    /// </summary>
    /// <param name="author"></param>
    /// <returns>Автор</returns>
    [Subscribe]
    public Author OnAuthorChanged([EventMessage] Author author) 
        => author;
}
