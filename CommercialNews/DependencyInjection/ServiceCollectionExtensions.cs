using Application.Interfaces;
using Application.Services;
using Infrastructure.Abstractions;
using Infrastructure.BackgroundServices;
using Infrastructure.Data.Helpers;
using Infrastructure.Data.Repositories;
using Infrastructure.Services;

namespace CommercialNews.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services) 
        {
            // Database, HttpClient
            services.AddScoped<IDatabaseHelper, DatabaseHelper>();
            services.AddScoped<IHttpClientService, HttpClientService>();

            // Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserVerificationRepository, UserVerificationRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserAuditRepository, UserAuditRepository>();
            services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();

            // Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserVerificationService, UserVerificationService>();
            services.AddScoped<IAuthService, AuthService>();

            // Token Generator
            services.AddScoped<ITokenGenerator, TokenGenerator>();

            // Register BackgroundService
            services.AddHostedService<VerificationTokenCleanupHostedService>();

            // Email Service
            services.AddScoped<IEmailService, EmailService>();

            //PasswordHash
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            //Token Generator
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();



            return services;

        }
    }
}
