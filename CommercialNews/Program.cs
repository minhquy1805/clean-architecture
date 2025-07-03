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

namespace CommercialNews
{
    public class Program
    {
        public static void Main(string[] args)
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

            // register services for dependency injection (di)
            builder.Services.AddSingleton(connectionString);
            builder.Services.AddProjectServices();

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

            //Build App
            var app = builder.Build();
            app.UseMiddleware<CommercialNews.Middleware.ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            

            //var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            //Console.WriteLine(key);

            //Use CORS 
            app.UseCors("AllowAll");

            // ✅ Rate Limiting Middleware
            app.UseIpRateLimiting();

            // ✅ Use Authentication & Authorization — ĐÚNG THỨ TỰ
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers
            app.MapControllers();


            //Run
            app.Run();
        }
    }
}
