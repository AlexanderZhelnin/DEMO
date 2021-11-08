using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Demo.GraphQl;
using Demo.Filters;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Http;
using HotChocolate.Execution;
using System.Threading;
using System.Security.Claims;
using System.Net.Http;
using Demo.HttpHandler;
using HotChocolate;

namespace Demo
{
    #region EnumSchemeFilter
    public class EnumSchemeFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum) return;

            var namevalues = new OpenApiArray();
            namevalues.AddRange(Enum.GetNames(context.Type).Select(name => new OpenApiString(name)));

            // NSwagStudio
            schema.Extensions.Add("x-enumNames", namevalues);

            // OpenAPI generator
            // schema.Extensions.Add("x-enum-varnames", namevalues);
        }
    }
    #endregion


    /** **/
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /** конфигурация **/
        public IConfiguration Configuration { get; }

        #region Конструктор
        /** **/
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion


        /** Конфигурация сервисов **/
        public void ConfigureServices(IServiceCollection services)
        {

            #region Добавляем логирование
            services.AddLogging(loggingBuilder =>
                {
                    //Вывод в консоль
                    loggingBuilder
                          .AddConsole()
                          //выводим команды SQL
                          .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);

                    //Выводим в окно отладки
                    loggingBuilder
                          .AddDebug();
                });
            #endregion

            #region Регистрируем сервисы
            //Добавляем в DI конвеер контекст базы данных, в режиме singleton
            services.AddSingleton<LongPollingQuery<Author>>();
            //Добавляем в DI конвеер контекст базы данных, в режиме новый для каждого запроса
            services.AddTransient<DemoContext>();
            //Добавляем в DI конвеер репозиторий, в режиме новый для каждого запроса
            services.AddTransient<DemoRepository>();
            //Добавляем в DI конвеер обрабочик запроса, в режиме новый для каждого запроса
            services.AddTransient<HttpTrackerHandler>();
            #endregion

            #region Добавляем конфигурацию авторизации
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, config =>
                    {
                        config.TokenValidationParameters = new TokenValidationParameters
                        {
                            ClockSkew = TimeSpan.FromSeconds(5),
                            ValidateAudience = false
                        };

                        config.RequireHttpsMetadata = false;
                        config.Authority = "http://localhost:8080/auth/realms/SAT/";
                        config.Audience = "DEMO";
                    });
            #endregion

            #region Регистрируем фабруку для HttpClient "auth"
            services
                    .AddHttpClient("auth")
                    .AddHttpMessageHandler<HttpTrackerHandler>();
            #endregion

            #region Настраиваем параметры для контроллеров
            services
                    .AddControllers(o =>
                    {
                        //Региструрем обработчик для фильтрации ошибок
                        o.Filters.Add(typeof(ExceptionFilter));
                    })
                    .AddNewtonsoftJson(o =>
                    {
                        // Праметры серелизации - не серелизуем лишнюю информацию
                        o.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
                        o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                        //o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    });
            //.AddJsonOptions(o =>
            //{
            //    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //}); 
            #endregion

            #region Региструруем OpenApi 
            services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo
                        {
                            Title = "Демонстрационное приложение для REST API",
                            Version = "v1",
                            Description = "REST, CORS, LongPolling, Routing, ...",
                            Contact = new OpenApiContact
                            {
                                Name = "Александр Желнин"

                            }
                            //, License =
                            //, TermsOfService = 
                        });

                        c.SchemaFilter<EnumSchemeFilter>();

                        c.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml"));
                    });
            #endregion

            #region Региструруем CORS
            services.AddCors(options =>
                {
                    options.AddPolicy("MyPolicy", builder =>
                    {
                        builder
                            //.AllowAnyOrigin()
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
            #endregion

            //Региструруем доступ к контесту запроса
            services.AddHttpContextAccessor();

            //региструруем кэширование в оперативной памяти
            services.AddMemoryCache();

            #region Entity
            services.AddPooledDbContextFactory<DemoContext>(ob =>
                {
#if SQLITE
                var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "gc.db") }.ToString());
#else
                    var connection = new Npgsql.NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection"));
#endif

                    connection.Open();
                    ob
#if SQLITE
                 .UseSqlite(connection)
#else
                 .UseNpgsql(connection)
#endif

                     .UseSnakeCaseNamingConvention()
                     .EnableSensitiveDataLogging(true);

                });
            #endregion

            #region MyRegion
            //#if SQLITE
            //            //Региструруем Entity для Sqlite
            //            services
            //               //.AddEntityFrameworkSqlite()
            //               .AddDbContext<DemoContext>
            //               (o =>
            //               {

            //                   var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db.db") }.ToString());

            //                   connection.Open();
            //                   o.UseSqlite(connection)
            //                    .UseSnakeCaseNamingConvention()
            //                    .EnableSensitiveDataLogging(true);
            //               });
            //#else
            //            //Региструруем Entity для PostgreSQL
            //            //services
            //            // .AddDbContext<DemoContext>(ob =>
            //            // {
            //            //     var connection = new Npgsql.NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection"));
            //            //     connection.Open();
            //            //     ob
            //            //      .UseNpgsql(connection)
            //            //      .UseSnakeCaseNamingConvention()
            //            //      .EnableSensitiveDataLogging(true);
            //            // });
            //#endif 
            #endregion

            #region Региструруем GraphQL
            services
                //подписки в памяти
                .AddInMemorySubscriptions()
                .AddGraphQLServer()
                //Интерсептор запроса
                .AddHttpRequestInterceptor<HttpRequestInterceptor>()
            #region Транзакции

#if !SQLITE
                //поддержка транзакций
                //SQLite не поддерживает транзакции
                .AddDefaultTransactionScopeHandler()
#endif
            #endregion
                //Авторизация
                .AddAuthorization()
                //Запросы
                .AddQueryType<Query>()
                //Изменения
                .AddMutationType<Mutation>()
                //Подписки
                .AddSubscriptionType<Subscription>()
                //.AddRemoteSchema("workes")
                //Запросы будут правильно транслироваться в SQL, не запрашиваем лишнее
                .AddProjections()
                //добавляем возможность фильтрации
                .AddFiltering()
                //добавляем возможность сортировки
                .AddSorting()
                //Добавляем функцию автоматических сохранённых запросов
                .UseAutomaticPersistedQueryPipeline()
                //Хранилище сохранённых запросов в оперативной памяти
                .AddInMemoryQueryStorage()
            #region Обработка ошибок
                .AddErrorFilter(er =>
                        {
                            switch (er.Exception)
                            {
                                case ArgumentException argexc:
                                    return ErrorBuilder.FromError(er)
                                    .SetMessage(argexc.Message)
                                    .SetCode("ArgumentException")
                                    .RemoveException()
                                    .ClearExtensions()
                                    .ClearLocations()
                                    .Build();
                                case DbUpdateException dbupdateexc:

                                    if (dbupdateexc.InnerException.Message.IndexOf("UNIQUE constraint failed") > -1)
                                        return ErrorBuilder.FromError(er)
                                       .SetMessage(dbupdateexc.InnerException.Message)
                                       .SetCode("UNIQUE constraint failed")
                                       .RemoveException()
                                       .ClearExtensions()
                                       .ClearLocations()
                                       .Build();

                                    break;
                            }
                            return er;
                        })
            #endregion
                ;
            #endregion

        }

        #region Configure
        /** Конфигурация midlware **/
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo v1"));
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseWebSockets();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("MyPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL("/graphql");
            });
        }
        #endregion
    }
}
