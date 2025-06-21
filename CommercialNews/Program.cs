
using Application.DTOs;
using CommercialNews.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace CommercialNews
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

           

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // get the connection string from appsettings.json
            string connectionString = builder.Configuration.GetValue<string>("Settings:ConnectionString")!;

            // register services for dependency injection (di)
            builder.Services.AddSingleton(connectionString);
            builder.Services.AddProjectServices();

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
