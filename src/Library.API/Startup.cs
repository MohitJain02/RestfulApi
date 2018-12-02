using AspNetCoreRateLimit;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Library.API
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(serviceOptions =>
            {
                serviceOptions.ReturnHttpNotAcceptable = true;
                serviceOptions.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                var xmlInputFormatter = new XmlDataContractSerializerInputFormatter();

                xmlInputFormatter.SupportedMediaTypes
                                .Add("application/vnd.marvin.authorwithdateofdateofdeath.full.xml");

                serviceOptions.InputFormatters.Add(xmlInputFormatter);

                var jsonInputFormatter = serviceOptions.InputFormatters
                                            .OfType<JsonInputFormatter>().FirstOrDefault();

                if(jsonInputFormatter != null)
                {
                    jsonInputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.author.full+json");
                    jsonInputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.authorwithdateofdeath.full+json");
                }

                var jsonOutputFormatter = serviceOptions.OutputFormatters
                                                .OfType<JsonOutputFormatter>().FirstOrDefault();

                if(jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                }
            })
             .AddJsonOptions(options =>
             {
                 options.SerializerSettings.ContractResolver =
                 new CamelCasePropertyNamesContractResolver();
             });


            // register the DbContext on the container, getting the connection string from
            // appSettings (note: use this during development; in a production environment,
            // it's better to store the connection string in an environment variable)
            var connectionString = Configuration["connectionStrings:libraryDBConnectionString"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));

            // register the repository
            services.AddScoped<ILibraryRepository, LibraryRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
             
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddResponseCaching(); // cache-store

            services.AddHttpCacheHeaders(expirationModelOptionsAction =>
            {
                expirationModelOptionsAction.MaxAge = 600;
            }, 
            validationModelOptionsAction =>
            {
                validationModelOptionsAction.MustRevalidate = true;
            }); // to register the Http Caching service in container


            // CORS Policy with GET methods allowed
            services.AddCors(options =>
            {
                options.AddPolicy("LibraryAPICorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin().WithMethods("GET");
                });
            });

            // Need memorycache to store the limit count and rules
            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Period = "5m",
                        Limit = 3,
                        Endpoint = "*"
                    }
                };
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {
            loggerFactory.AddConsole();

            loggerFactory.AddDebug(LogLevel.Information);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exception = context.Features.Get<IExceptionHandlerFeature>();

                        var logger = loggerFactory.CreateLogger("Global exception logger");

                        logger.LogError(500,
                            exception.Error,
                            exception.Error.Message);

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault exception, Try Again!!!");
                    });
                });


                AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Author, AuthorDto>()
                                         .ForMember(dest => dest.Name,
                                                     opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                                         .ForMember(dest => dest.Age,
                                                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

                    // the properties which are not present in the destination and are 
                    // present in the source then they will be marked as null
                    cfg.CreateMap<Book, BooksDto>();

                    cfg.CreateMap<AuthorForCreateDto, Author>();

                    cfg.CreateMap<AuthorForCreateWithDateOfDeathDto, Author>();

                    cfg.CreateMap<BookCreationDto, Book>();

                    cfg.CreateMap<UpdateBookDto, Book>();

                    cfg.CreateMap<Book, UpdateBookDto>();

                });

                libraryContext.EnsureSeedDataForContext();

                app.UseIpRateLimiting();

                app.UseResponseCaching();

                app.UseHttpCacheHeaders(); // to register the caching middleware in the aspnet core pipeline
                                           // before mvc 
                app.UseMvc();
            }
        }
    }
}
