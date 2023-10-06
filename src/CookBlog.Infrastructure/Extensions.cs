using CookBlog.Api.Application.Abstractions;
using CookBlog.Api.Core.Abstractions;
using CookBlog.Api.Infrastructure.Auth;
using CookBlog.Api.Infrastructure.DAL;
using CookBlog.Api.Infrastructure.Exceptions;
using CookBlog.Api.Infrastructure.Logging;
using CookBlog.Api.Infrastructure.Security;
using CookBlog.Api.Infrastructure.Time;
using CookBlog.Core.Abstractions;
using CookBlog.Infrastructure.DAL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CookBlog.Tests.Integration")]
[assembly: InternalsVisibleTo("CookBlog.Tests.Unit")]
namespace CookBlog.Api.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<AppOptions>(configuration.GetRequiredSection("app"));
        services.AddSingleton<ExceptionMiddleware>();
        services.AddHttpContextAccessor();

        services
            .AddMSql(configuration)
            .AddSingleton<IClock, Clock>()
            .AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddCustomLogging();
        services.AddSecurity();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations();
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CookBlog API",
                Version = "v1"
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        var infrastructureAssembly = typeof(AppOptions).Assembly;

        services.Scan(s => s.FromAssemblies(infrastructureAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddAuth(configuration);

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwagger();
        app.UseReDoc(reDoc =>
        {
            reDoc.RoutePrefix = "docs";
            reDoc.SpecUrl("/swagger/v1/swagger.json");
            reDoc.DocumentTitle = "CookBlog API";
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("Open");
        app.MapControllers();

        return app;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);

        return options;
    }
}
