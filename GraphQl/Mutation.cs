﻿using Demo.Model;
using HotChocolate;
using HotChocolate.Subscriptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.GraphQl
{
    /// <summary>
    /// Изменения GraphQL
    /// Главное отличие в том что подзапросы выполняются последовательно, в том же порядке что указано в запросе
    /// </summary>
    public class Mutation
    {       
        #region Create
        /// <summary>
        /// Создание автора
        /// </summary>
        /// <param name="author">Создаваемый автор</param>
        /// <param name="ctx">Контекст базы данных Entity</param>
        /// <returns>Автор</returns>        
        public Author Create(Author author, [Service] DemoContext ctx)
        {
            ctx.Add(author);
            ctx.SaveChanges();
            return author;
        }
        #endregion

        #region Update
        /// <summary>
        /// Обновление автора
        /// </summary>
        /// <param name="author">Обновляемый автор</param>
        /// <param name="ctx">Контекст базы данных Entity</param>
        /// <returns>Автор</returns>        
        public Author Update(Author author, [Service] DemoContext ctx)
        {
            ctx.Update(author);
            ctx.SaveChanges();
            return author;
        }
        #endregion

        #region CreateOrUpdate
        /// <summary>
        /// Создать или обновить автора тут логика такая, если Id равер 0, то это однозначно новый автор
        /// </summary>
        /// <param name="ctx">Контекст базы данных Entity</param>
        /// <param name="author">Создать или обновить автора</param>
        /// <param name="sender"></param>
        /// <returns>Автор</returns>        
        public async Task<Author> CreateOrUpdate(Author author, [Service] DemoContext ctx, [Service] ITopicEventSender sender)
        {
            if (author.Id == 0 || !ctx.Authors.Any(a => a.Id == author.Id))
                ctx.Add(author);
            else
                ctx.Update(author);

            ctx.SaveChanges();

            await sender.SendAsync(nameof(Subscription.OnAuthorChanged), author);
            return author;
        }
        #endregion

        /// <summary>
        /// Генерация ошибки для проверки транзакции
        /// </summary>      
        /// <returns>Автор</returns>        
        public Author ThrowError()
        {
            throw new Exception("Специально сгенерированная ошибка");
        }

    }
}
