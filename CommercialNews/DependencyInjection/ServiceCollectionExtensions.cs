using Application.Interfaces;
using Application.Services;
using Infrastructure.Abstractions;
using Infrastructure.Data.Helpers;
using Infrastructure.Data.Repositories;
using Infrastructure.Services;

namespace CommercialNews.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services) 
        {
            //Add DatabaseHelper
            services.AddScoped<IDatabaseHelper, DatabaseHelper>();

            //Add HttpClientService
            services.AddScoped<IHttpClientService, HttpClientService>();

            // Add your repository & business service DI here:
            // services.AddScoped<INewsRepository, NewsRepository>();
            // services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
