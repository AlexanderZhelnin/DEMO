using Demo.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo;

/** Фоновый процесс расчёта среднего колличества отзывов */
public class BookAVG : BackgroundService
{
    private readonly ILogger<BookAVG> _logger;
    private readonly DemoContext _ctx;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Конструктор
    /// </summary>    
    /// <param name="logger"></param>
    /// <param name="ctx">контекст базы данных</param>
    public BookAVG(ILogger<BookAVG> logger, DemoContext ctx)
    {        
        _logger = logger;
        _ctx = ctx;
    }

    /// <summary>
    /// Выполнение фонового процесса
    /// </summary>
    /// <param name="stoppingToken">токен завершения</param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug(_ctx.BookDetails.Average(db => db.Reviews).ToString());

            await Task.Delay(_interval, stoppingToken);
        }       
        
    }
    
}

