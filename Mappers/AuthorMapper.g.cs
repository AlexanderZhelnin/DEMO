using System;
using System.Linq.Expressions;
using Demo;
using Demo.Models;

namespace Demo
{
    public partial class AuthorMapper : IAuthorMapper
    {
        public Expression<Func<Author, AuthorDTO>> AuthorProjection => p1 => new AuthorDTO()
        {
            Id = p1.Id,
            Name = p1.Name,
            Name1 = p1.Name + "1"
        };
        public AuthorDTO MapTo(Author p2)
        {
            return p2 == null ? null : new AuthorDTO()
            {
                Id = p2.Id,
                Name = p2.Name,
                Name1 = p2.Name + "1"
            };
        }
        public AuthorDTO MapTo(Author p3, AuthorDTO p4)
        {
            if (p3 == null)
            {
                return null;
            }
            AuthorDTO result = p4 ?? new AuthorDTO();
            
            result.Id = p3.Id;
            result.Name = p3.Name;
            result.Name1 = p3.Name + "1";
            return result;
            
        }
    }
}