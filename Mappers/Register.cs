using Demo.Models;
using Mapster;

namespace Demo.Mappers
{
    /// <summary>
    /// Регистрация сопоставления
    /// </summary>
    public class RegisterMapper : IRegister
    {
        /** */
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Author, AuthorDTO>()
                .Map(adto => adto.Name1, a => a.Name + "1")                
                .RequireDestinationMemberSource(true);
        }
    }
}
