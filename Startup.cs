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
        /** ������������ **/
        public IConfiguration Configuration { get; }

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
            //��������� �����������
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder
                  //����� � �������
                  .AddConsole()
                  //������� ������� SQL
                  .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
                loggingBuilder.AddDebug();
            });

            //��������� � DI ������� �������� ���� ������, � ������ singleton
            services.AddSingleton<LongPollingQuery<Author>>();
            //��������� � DI ������� �������� ���� ������, � ������ ����� ��� ������� �������
            services.AddTransient<DemoContext>();
            //��������� � DI ������� �����������, � ������ ����� ��� ������� �������
            services.AddTransient<DemoRepository>();
            //��������� � DI ������� ��������� �������, � ������ ����� ��� ������� �������
            services.AddTransient<HttpTrackerHandler>();

            //��������� ������������ �����������
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

            //������������ ������� ��� HttpClient "auth"
            services
                .AddHttpClient("auth")
                .AddHttpMessageHandler<HttpTrackerHandler>();

            //������������ Entity ��� Sqlite
            services
               .AddEntityFrameworkSqlite()
               .AddDbContext<DemoContext>
               (o =>
               {

                   var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db.db") }.ToString());

                   connection.Open();
                   o.UseSqlite(connection)
                    .UseSnakeCaseNamingConvention()
                    .EnableSensitiveDataLogging(true);
               });

            //����������� ��������� ��� ������������
            services
                .AddControllers(o =>
                {
                    //����������� ���������� ��� ���������� ������
                    o.Filters.Add(typeof(ExceptionFilter));
                })
                .AddNewtonsoftJson(o =>
                {
                    // �������� ����������� - �� ���������� ������ ����������
                    o.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
                    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                    //o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            //.AddJsonOptions(o =>
            //{
            //    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //});

            //������������ OpenApi 
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "���������������� ���������� ��� REST API",
                        Version = "v1",
                        Description = "REST, CORS, LongPolling, Routing, ...",
                        Contact = new OpenApiContact
                        {
                            Name = "��������� ������"

                        }
                        //, License =
                        //, TermsOfService = 
                    });

                    c.SchemaFilter<EnumSchemeFilter>();

                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml"));
                });

            //������������ CORS
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

            //������������ ������ � �������� �������
            services.AddHttpContextAccessor();

            //������������ ����������� � ����������� ������
            services.AddMemoryCache();

            //������������ GraphQL
            services
                .AddGraphQLServer()
                //����������� �������
                .AddHttpRequestInterceptor<HttpRequestInterceptor>()
                //�����������
                .AddAuthorization()
                //�������
                .AddQueryType<Query>()
                //���������
                .AddMutationType<Mutation>()
                //.AddRemoteSchema("workes")
                //������� ����� ��������� ��������������� � SQL, �� ����������� ������
                .AddProjections()
                //��������� ����������� ����������
                .AddFiltering()
                //��������� ����������� ����������
                .AddSorting()
                //��������� ������� �������������� ���������� ��������
                .UseAutomaticPersistedQueryPipeline()
                //��������� ���������� �������� � ����������� ������
                .AddInMemoryQueryStorage();
        }

        #region Configure
        /** ������������ midlware **/
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
