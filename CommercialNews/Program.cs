
using CommercialNews.DependencyInjection;

namespace CommercialNews
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // get the connection string from appsettings.json
            string connectionString = builder.Configuration.GetValue<string>("Settings:ConnectionString")!;

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // register services for dependency injection (di)
            builder.Services.AddSingleton(connectionString);
            builder.Services.AddProjectServices();

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
