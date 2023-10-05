using Azure.Storage.Blobs;
using CookBlog.Api.Application.Abstractions;
using CookBlog.Api.Core.Repositories;
using CookBlog.Api.Infrastructure.DAL.Decorators;
using CookBlog.Api.Infrastructure.DAL.Repositories;
using CookBlog.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CookBlog.Api.Infrastructure.DAL;

internal static class Extensions
{
    private const string OptionsSectionName = "MSql";
    private const string OptionsRedisName = "Redis";
    private const string OptionsExtensionFileName = "ExtensionsFile";
    private const string OptionsAzureName = "Azure";

    public static IServiceCollection AddMSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureOptions>(configuration.GetRequiredSection(OptionsAzureName));
        var azureOptions = configuration.GetOptions<AzureOptions>(OptionsAzureName);
        services.AddSingleton(azureOptions);

        services.Configure<ExtensionFileOptions>(configuration.GetRequiredSection(OptionsExtensionFileName));
        var extensionsFile = configuration.GetOptions<ExtensionFileOptions>(OptionsExtensionFileName);
        services.AddSingleton(extensionsFile);

        services.Configure<MSqlOptions>(configuration.GetRequiredSection(OptionsSectionName));
        var mSqlOptions = configuration.GetOptions<MSqlOptions>(OptionsSectionName);
        services.AddDbContext<MyCookBlogDbContext>(x => x.UseSqlServer(mSqlOptions.ConnectionString));

        services.Configure<RedisOptions>(configuration.GetRequiredSection(OptionsRedisName));
        var redisOptions = configuration.GetOptions<RedisOptions>(OptionsRedisName);
        services.AddStackExchangeRedisCache(redisCacheOptions => { redisCacheOptions.Configuration = redisOptions.Url; });

        services.AddScoped<IUserRepository, MSqlUserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddHostedService<DatabaseInitializer>();
        services.AddScoped<IUnitOfWork, MSqlUnitOfWork>();
        services.AddScoped<ITagRepository, TagRepository>();

        //      services.Decorate<ITagRepository, CacheTagRepository>();
        services.TryDecorate(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));

        return services;
    }
}