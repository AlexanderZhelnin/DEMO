using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Filters;

/// <summary>
/// Фильтр ошибок
/// </summary>
public class ExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// Обработчик фильтра ошибок
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ArgumentException e:
                context.Result = new NotFoundObjectResult(e.Message);
                break;
            case InvalidOperationException e:
                context.Result = new BadRequestObjectResult(e.Message);
                break;
        }
    }
}
