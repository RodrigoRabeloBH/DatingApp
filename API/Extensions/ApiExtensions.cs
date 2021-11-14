using System.Text;
using API.Data;
using API.Data.Repository;
using API.Helpers;
using API.Interfaces;
using API.Interfaces.Repository;
using API.Mapping;
using API.Middleware;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddApiExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            services.AddScoped<LogUserActivity>();

            services.AddCors(option =>
            {
                option.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            AddSwaggerDocumentation(services);

            AddServicesExtensions(services);

            AddDatabaseExtensions(services, configuration);

            AddAuthenticationJwt(services, configuration);

            return services;
        }
        public static IApplicationBuilder UseApiExtensions(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            UseSwaggerDocumentation(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            return app;
        }
        private static void AddSwaggerDocumentation(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DatingApp API", Version = "v1" });
                });
        }
        private static void AddServicesExtensions(IServiceCollection services)
        {
            services.AddTransient<IAccountServices, AccountServices>();

            services.AddScoped<IPhotoService, PhotoService>();
        }
        private static void AddDatabaseExtensions(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options => options.UseSqlServer(config.GetConnectionString("SqlServer")));

            services.AddTransient<IUserRepository, UserRepository>();
        }
        private static void UseSwaggerDocumentation(IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DatingApp API v1"));
        }
        private static void AddAuthenticationJwt(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}