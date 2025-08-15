using System.Text;
using DataWarehouse.API.Repositories.Implementation.Companies;
using DataWarehouse.API.Repositories.Implementation.Users;
using DataWarehouse.API.Repositories.Interfaces.Companies;
using DataWarehouse.API.Repositories.Interfaces.Users;
using DataWarehouse.API.Services.Implementation.Companies;
using DataWarehouse.API.Services.Implementation.Users;
using DataWarehouse.API.Services.Interfaces.Companies;
using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.API.Utils.Hash;
using DataWarehouse.API.Utils.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DataWarehouse.API.GraphQL;
using DataWarehouse.API.GraphQL.Companies;
using DataWarehouse.API.GraphQL.Users;
using DataWarehouse.API.Utils.Redis;

namespace DataWarehouse.API.Extensions;

public static class ServiceRegistrationExtensions
{
    public static void RegisterCoreServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });
        services.AddScoped<IHashUtility, HashUtility>();
        services.AddSingleton<IJwtUtility, JwtUtility>();
        services.AddHttpContextAccessor();
        services.AddAuthorization();
    }

    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
    }

    public static void RegisterBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddSingleton<RedisHelper>();
    }

    public static void RegisterJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"] ?? throw new Exception("JWT Key missing"));

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; 
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
    }

    public static void RegisterCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontendOnly", builder =>
            {
                if (origins.Length > 0)
                {
                    builder.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
                else
                {
                    // No credentials when allowing any origin
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });
    }
    
    public static void RegisterGraphQl(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddGraphQLServer()
            .AddAuthorization()                 
            .AddQueryType<Query>()              
            .AddMutationType<Mutation>()        
            .AddTypeExtension<UserQueries>()   
            .AddTypeExtension<UserMutations>()  
            .AddTypeExtension<CompanyQueries>() 
            .AddTypeExtension<CompanyMutations>()
            .AddType<CompanyType>() 
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .UseField<GraphQlExceptionMiddleware>()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true);
    }
}