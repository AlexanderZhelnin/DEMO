using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo;

/** Сервер TCP */
public class TCPServer : BackgroundService
{
    private readonly ILogger<TCPServer> _logger;
    private const int port = 8888;

    /// <summary>
    /// Конструктор
    /// </summary>    
    /// <param name="logger"></param>    
    public TCPServer(ILogger<TCPServer> logger)
    {
        _logger = logger;

    }

    /// <summary>
    /// Выполнение фонового процесса
    /// </summary>
    /// <param name="stoppingToken"></param>    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener server = null;
        try
        {
            server = new TcpListener(IPAddress.Any, port);
            
            // запуск слушателя
            server.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Ожидание подключений... ");

                // получаем входящее подключение
                using var client = server.AcceptTcpClient();
                _logger.LogTrace("Подключен клиент. Выполнение запроса...");

                // получаем сетевой поток для чтения и записи
                using var stream = client.GetStream();

                // сообщение для отправки клиенту
                var response = "Привет мир";
                // преобразуем сообщение в массив байтов
                var data = Encoding.UTF8.GetBytes(response);
                
                // отправка сообщения
                await stream.WriteAsync(new ReadOnlyMemory<byte>(data), stoppingToken);
                _logger.LogTrace($"Отправлено сообщение: {response}");

            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
        finally
        {
            server?.Stop();
        }
    }

}


