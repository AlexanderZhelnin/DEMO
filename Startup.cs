using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Demo.Model;
using Microsoft.EntityFrameworkCore;
using Demo.GraphQl;
using Demo.Filters;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Demo.HttpHandler;
using HotChocolate;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using HotChocolate.Types.Pagination;
using Demo.Health;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting.Internal;

namespace Demo;

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

//public class AuthorFilterType : FilterInputType<Author>
//{
//    protected override void Configure(
//        IFilterInputTypeDescriptor<Author> descriptor)
//    {
//        descriptor.BindFieldsExplicitly();
//        descriptor.Field(f => f.Name).Type<AuthorOperationFilterInput>();
//    }
//}

//public class AuthorOperationFilterInput : StringOperationFilterInputType
//{
//    protected override void Configure(IFilterInputTypeDescriptor descriptor)
//    {
//        descriptor.Operation(DefaultFilterOperations.Equals);
//        //descriptor.Operation(DefaultFilterOperations.NotEquals);
//    }
//}
public class DateTimeModelBinderProvider : IModelBinderProvider
{
    // You could make this a property to allow customization
    internal static readonly DateTimeStyles SupportedStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces;

    /// <inheritdoc />
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var modelType = context.Metadata.UnderlyingOrModelType;
        var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
        if (modelType == typeof(DateTime))
        {
            return new UtcAwareDateTimeModelBinder(SupportedStyles, loggerFactory);
        }

        return null;
    }
}
public class UtcAwareDateTimeModelBinder : IModelBinder
{
    private readonly DateTimeStyles _supportedStyles;
    private readonly ILogger _logger;

    public UtcAwareDateTimeModelBinder(DateTimeStyles supportedStyles, ILoggerFactory loggerFactory)
    {
        if (loggerFactory == null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }

        _supportedStyles = supportedStyles;
        _logger = loggerFactory.CreateLogger<UtcAwareDateTimeModelBinder>();
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            // no entry
            return Task.CompletedTask;
        }

        var modelState = bindingContext.ModelState;
        modelState.SetModelValue(modelName, valueProviderResult);

        var metadata = bindingContext.ModelMetadata;
        var type = metadata.UnderlyingOrModelType;

        var value = valueProviderResult.FirstValue;
        var culture = valueProviderResult.Culture;

        object model;
        if (string.IsNullOrWhiteSpace(value))
        {
            model = null;
        }
        else if (type == typeof(DateTime))
        {
            // You could put custom logic here to sniff the raw value and call other DateTime.Parse overloads, e.g. forcing UTC
            model = DateTime.Parse(value, culture, _supportedStyles);
        }
        else
        {
            // unreachable
            throw new NotSupportedException();
        }

        // When converting value, a null model may indicate a failed conversion for an otherwise required
        // model (can't set a ValueType to null). This detects if a null model value is acceptable given the
        // current bindingContext. If not, an error is logged.
        if (model == null && !metadata.IsReferenceOrNullableType)
        {
            modelState.TryAddModelError(
                modelName,
                metadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                    valueProviderResult.ToString()));
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Success(model);
        }

        return Task.CompletedTask;
    }
}

#region SettingsWatchdog
/** ������������ ��������� ������������ */
public class SettingsWatchdog
{
    private readonly IOptionsMonitor<OAUTHSettings> _settings;


    public SettingsWatchdog(IOptionsMonitor<OAUTHSettings> settings)
    {
        _settings = settings;
        _settings.OnChange((s) =>
        {
            Console.WriteLine("������� ������������� �����������");
        });
    }
}
#endregion


#region OAUTHSettings
public class OAUTHSettings
{
    public string OAUTH_PATH { get; set; }
    public string OAUTH_CLIENT_ID { get; set; }
}
#endregion


/** **/
[ExcludeFromCodeCoverage]
public class Startup
{
    /** ������������ **/
    public IConfiguration Configuration { get; }

    #region WriteHealthCheckResponse
    /** ������� ������ ���������� �������� ����������������� */
    private static Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = "application/json";
        var json = new JObject(
        new JProperty("status", result.Status.ToString()),
        new JProperty("results", new JObject(result.Entries.Select(pair =>
        new JProperty(pair.Key, new JObject(
        new JProperty("status", pair.Value.Status.ToString()),
        new JProperty("description", pair.Value.Description),
        new JProperty("data", new JObject(pair.Value.Data.Select(
        p => new JProperty(p.Key, p.Value))))))))));
        return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
    }
    #endregion

    #region �����������
    /** **/
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    #endregion


    /** ������������ �������� **/
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<OAUTHSettings>(Configuration.GetSection("Authentication"));

        //services.AddSingleton<AppSettingsWrapper>();
        //(sp =>
        //{
        //    return new AppSettingsWrapper(sp.GetService<IOptionsMonitor<Settings>>());
        //});

        #region ��������� �����������
        services.AddLogging(loggingBuilder =>
            {
                //����� � �������
                loggingBuilder
                      .AddConsole()
                      //������� ������� SQL
                      .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);

                //������� � ���� �������
                loggingBuilder
                      .AddDebug();
            });
        #endregion

        #region ������������ �������
        //������������ ��������� ������������
        services.AddSingleton<SettingsWatchdog>();
        //��������� � DI ������� �������� ���� ������, � ������ singleton
        services.AddSingleton<LongPollingQuery<Author>>();
        //��������� � DI ������� �������� ���� ������, � ������ ����� ��� ������� �������
        services.AddTransient<DemoContext>();
        //��������� � DI ������� �����������, � ������ ����� ��� ������� �������
        services.AddTransient<DemoRepository>();
        //��������� � DI ������� ��������� �������, � ������ ����� ��� ������� �������
        services.AddTransient<HttpTrackerHandler>();

        #endregion

        #region ��������� ������������ �����������
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, config =>
                {
                    var aoptions = Configuration
                        .GetSection("Authentication")
                        .Get<OAUTHSettings>();

                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.FromSeconds(5),
                        ValidateAudience = false
                    };
                    config.RequireHttpsMetadata = false;
                    // ������ OAUTH 2.0 
                    config.Authority = Configuration["Authentication:OAUTH_PATH"];
                    // �������� ��� �������
                    config.Audience = Configuration["Authentication:OAUTH_CLIENT_ID"];
                });
        #endregion

        #region ������������ ������� ��� HttpClient "auth"
        services
                .AddHttpClient("auth")
                .AddHttpMessageHandler<HttpTrackerHandler>();
        #endregion

        #region ����������� ��������� ��� ������������
        services
                .AddControllers(o =>
                {
                    o.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
                    //����������� ���������� ��� ���������� ������
                    o.Filters.Add(typeof(ExceptionFilter));
                })
                .AddNewtonsoftJson(o =>
                {
                    // �������� ����������� - �� ���������� ������ ����������
                    o.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
                    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                    //o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                })
                ;
        //.AddJsonOptions(o =>
        //{
        //    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        //}); 
        #endregion

        #region ������������ OpenApi 
        services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "���������������� ����������",
                        Version = "v1",
                        Description = "REST, CORS, LongPolling, Routing, GraphQL, NLog ...",
                        Contact = new OpenApiContact
                        {
                            Name = "��������� ������"
                        }
                        //, License =
                        //, TermsOfService = 
                    });

                    c.SchemaFilter<EnumSchemeFilter>();

                    c.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml"));
                });
        #endregion

        #region ������������ CORS
        services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        //.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        #endregion

        //������������ ������ � �������� �������
        services.AddHttpContextAccessor();

        //������������ ����������� � ����������� ������
        services.AddMemoryCache();

        #region Entity
        services.AddPooledDbContextFactory<DemoContext>(ob =>
            {
#if SQLITE
                var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "db.db") }.ToString());
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
        //            //������������ Entity ��� Sqlite
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
        //            //������������ Entity ��� PostgreSQL
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

        #region ������������ GraphQL
        services
            //�������� � ������
            .AddInMemorySubscriptions()
            .AddGraphQLServer()
            //����������� �������
            .AddHttpRequestInterceptor<HttpRequestInterceptor>()
        #region ����������

#if !SQLITE
            //��������� ����������
            //SQLite �� ������������ ����������
            .AddDefaultTransactionScopeHandler()
#endif
        #endregion
            //�����������
            .AddAuthorization()
            //�������
            .AddQueryType<Query>()
            //���������
            .AddMutationType<Mutation>()
            //��������
            .AddSubscriptionType<Subscription>()
            //.AddRemoteSchema("workes")
            //������� ����� ��������� ��������������� � SQL, �� ����������� ������
            .AddProjections()
            //��������� ����������� ����������
            .AddFiltering()
            //��������� ����������� ����������
            .AddSorting()
            .SetPagingOptions(new PagingOptions
            {
                MaxPageSize = 500,
                DefaultPageSize = 100,

                //AllowBackwardPagination = false
            })

            //��������� ������� �������������� ���������� ��������
            .UseAutomaticPersistedQueryPipeline()
            //��������� ���������� �������� � ����������� ������
            .AddInMemoryQueryStorage()
        #region ��������� ������
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

        #region ������������ ������� ������
        //services.AddHostedService<BookAVG>();
        //services.AddHostedService<TCPServer>(); 
        #endregion

        services.AddHealthChecks()
            .AddCheck<CustumHealthCheck>("API")
            .AddNpgSql(Configuration.GetConnectionString("DefaultConnection"));
    }

    #region Configure
    /** ������������ midlware **/
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SettingsWatchdog swd)
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
            endpoints.MapGet("/", () => "���������������� ������ ��������� ������");
            endpoints.MapControllers();
            endpoints.MapGraphQL("/graphql");
        });

        app.UseHealthChecks("/health", new HealthCheckOptions { ResponseWriter = WriteHealthCheckResponse });
    }
    #endregion
}
