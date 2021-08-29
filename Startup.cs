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

    #region HttpRequestInterceptor
    public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
    {
        public override ValueTask OnCreateAsync(HttpContext context,
            IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder,
            CancellationToken cancellationToken)
        {
            var identity = new ClaimsIdentity();
            var rolesv = context.User.FindFirstValue("realm_access");
            if (rolesv != null)
            {
                dynamic roles = Newtonsoft.Json.Linq.JObject.Parse(rolesv);
                foreach (var r in roles.roles)
                    identity.AddClaim(new Claim(ClaimTypes.Role, r.Value));
            }

            var namev = context.User.FindFirstValue("preferred_username");
            if (namev != null)
                identity.AddClaim(new Claim(ClaimTypes.Name, namev));


            //

            context.User.AddIdentity(identity);

            return base.OnCreateAsync(context, requestExecutor, requestBuilder,
                cancellationToken);
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
            services.AddSingleton<LongPollingQuery<Author>>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole()
                    .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
                loggingBuilder.AddDebug();
            });

            services.AddTransient<DemoContext>();
            services.AddTransient<DemoRepository>();


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

            services.AddHttpClient("workes", c => c.BaseAddress = new Uri("https://localhost:5051/graphql"));

            services
               .AddEntityFrameworkSqlite()
               .AddDbContext<DemoContext>
               (o =>
               {
                   var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db.db") }.ToString());

                   connection.Open();
                   o.UseSqlite(connection).EnableSensitiveDataLogging(true);
               });

            services
                .AddControllers(o =>
                {
                    o.Filters.Add(typeof(ExceptionFilter));
                })
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;

                    //o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            //.AddJsonOptions(o =>
            //{
            //    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //});

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

                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml"));
                });

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

            services.AddHttpContextAccessor();

            services.AddMemoryCache();

            services
                .AddGraphQLServer()
                .AddHttpRequestInterceptor<HttpRequestInterceptor>()
                .AddAuthorization()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                //.AddRemoteSchema("workes")
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .UseAutomaticPersistedQueryPipeline()
                .AddInMemoryQueryStorage();
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
