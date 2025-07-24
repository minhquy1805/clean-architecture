using CommercialNews.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using AspNetCoreRateLimit;
using Infrastructure.Metadata.Bootstrap;
using Application.Filters;
using Application.Validators.Users;
using Application.DTOs.Auth.Jwt;
using Infrastructure.Seeding;
using Application.Interfaces.Services.AccessControl;
using Infrastructure.Authorization;
using Domain.Constants;
using Infrastructure.Repositories.Mongo;
using Prometheus;
using Infrastructure.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Application.Interfaces.Messaging;
using Application.DTOs.Email;

namespace CommercialNews
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<PagingDefaultsFilter>(); // ✅ Global Action Filter cho Paging
            });

            builder.Services.AddScoped<PagingDefaultsFilter>();

            // ✅ FluentValidation v11+
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(typeof(UserRegisterDtoValidator).Assembly);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // get the connection string from appsettings.json
            string connectionString = builder.Configuration.GetValue<string>("Settings:ConnectionString")!;

            // get the connection string from appsettings.json mongoddb
            builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

            // register services for dependency injection (di)
            builder.Services.AddSingleton<MongoDbContext>();

            // Optional: tiện inject trực tiếp IMongoDatabase nếu cần
            builder.Services.AddScoped(sp => sp.GetRequiredService<MongoDbContext>().Database);

            builder.Services.Configure<DefaultAdminSettings>(
                 builder.Configuration.GetSection("DefaultAdmin"));


            // register services for dependency injection (di)
            builder.Services.AddSingleton(connectionString);
            builder.Services.AddProjectServices(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            //1 Bind JWT Settings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            //2 Add Authentication + JWT Bearer
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
            });

            // Khởi tạo Metadata (rất quan trọng)
            MetadataBootstrapper.Initialize();

            // Add AspNetCoreRateLimit
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis:ConnectionString"];
            });

            //Add Authorization
            builder.Services.AddAuthorization();

            //Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin() // or .WithOrigins("https://example.com") to allow specific origins
                               .AllowAnyMethod()  // Allows GET, POST, PUT, DELETE, etc.
                               .AllowAnyHeader(); // Allows any header
                    });
            });

            builder.Services.AddAuthorization(options =>
            {
                foreach (var permission in PermissionConstants.All)
                {
                    options.AddPolicy(permission, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

            builder.Services.AddPermissionPolicies();

            builder.Services.AddProjectHealthChecks(builder.Configuration);

            //Build App
            var app = builder.Build();
            app.UseMiddleware<CommercialNews.Middleware.ExceptionMiddleware>();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // ✅ Gọi Seeder
                var roleSeeder = services.GetRequiredService<RoleSeederService>();
                await roleSeeder.SeedDefaultRolesAsync();

                var permissionSeeder = services.GetRequiredService<PermissionSeederService>();
                await permissionSeeder.SeedDefaultPermissionsAsync();

                var userRoleSeeder = services.GetRequiredService<UserRoleSeederService>();
                await userRoleSeeder.SeedAdminUserRoleAsync();

                var rolePermissionSeeder = services.GetRequiredService<RolePermissionSeederService>();
                await rolePermissionSeeder.SeedPermissionsToAdminAsync();
                await rolePermissionSeeder.SeedPermissionsToUserAsync();
                await rolePermissionSeeder.SeedPermissionsToModeratorAsync();

                // ✅ Lấy danh sách quyền sau khi seed
                var permissionService = services.GetRequiredService<IPermissionService>();
                var permissions = await permissionService.GetAllAsync();
                
            }

            // Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // CORS
            app.UseCors("AllowAll");

            // Rate limiting
            app.UseIpRateLimiting();

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseHttpMetrics();
            // 👉 Đây là cách chuẩn mới (không cần UseRouting + UseEndpoints)
            app.MapControllers();

            app.MapHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapMetrics(); // /metrics cho Prometheus


            app.Run();

        }
    }
}
