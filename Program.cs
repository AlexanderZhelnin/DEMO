using System;
using System.Diagnostics.CodeAnalysis;
using Demo.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace Demo
{
    /** */
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /** */
        public static void Main(string[] args)
        {

            var logger = NLogBuilder
                .ConfigureNLog("nlog.config")
                .GetCurrentClassLogger();
            try
            {
                logger.Debug("Инициализация программы");
                #region Инициализация программы
                var host = CreateHostBuilder(args).Build();

#if !SQLITE
                using (var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DemoContext>();
                    db.Database.Migrate();
                }
#endif 
                #endregion

                host.Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Закрытие программы");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        /** */
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .UseNLog();
    }
}