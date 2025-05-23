
using LinkLeaf.Api.Data;
using LinkLeaf.Api.Repositories;
using LinkLeaf.Api.Security;
using LinkLeaf.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace LinkLeaf.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppRepositoriesAndServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );



        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenHasher, TokenHasher>();
        services.AddScoped<IBookmarksRepository, BookmarksRepository>();

        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}
