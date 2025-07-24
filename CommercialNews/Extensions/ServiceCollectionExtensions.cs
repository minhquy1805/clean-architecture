using Application.Interfaces;
using Application.Interfaces.Common;
using Application.Interfaces.Messaging;
using Application.Interfaces.Messaging.Consumers;
using Application.Interfaces.Messaging.Handlers;
using Application.Interfaces.Messaging.Producers;
using Application.Interfaces.Redis;
using Application.Interfaces.Redis.Caching;
using Application.Interfaces.Redis.Tracking;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.AccessControl;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Interfaces.Services.AccessControl;
using Application.Interfaces.Slack;
using Application.Services;
using Application.Services.Common;
using Infrastructure.BackgroundServices;
using Infrastructure.Data.Helpers;
using Infrastructure.Database.Abstractions;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Handlers;
using Infrastructure.Messaging.Producers;
using Infrastructure.Redis.Caching;
using Infrastructure.Redis.Tracking;
using Infrastructure.Repositories.Mongo.Users;
using Infrastructure.Repositories.SQL.Permissions;
using Infrastructure.Repositories.SQL.RolePermissions;
using Infrastructure.Repositories.SQL.Roles;
using Infrastructure.Repositories.SQL.UserRoles;
using Infrastructure.Repositories.SQL.Users;
using Infrastructure.Seeding;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Email;
using Infrastructure.Services.Http;
using Infrastructure.Services.Slack;
using StackExchange.Redis;


namespace CommercialNews.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration config)
        {
            // ========= ✅ Database, HttpClient =========
            services.AddScoped<IDatabaseHelper, DatabaseHelper>();
            services.AddHttpClient();
            services.AddScoped<IHttpClientService, HttpClientService>();

            // ========= ✅ Repositories =========
            services.AddScoped<ILoginHistoryRepository, MongoLoginHistoryRepository>();
            services.AddScoped<IUserAuditRepository, MongoUserAuditRepository>();
            services.AddScoped<IUserRepository, SqlUserRepository>();
            services.AddScoped<IUserRoleRepository, SqlUserRoleRepository>();
            services.AddScoped<IUserVerificationRepository, SqlUserVerificationRepository>();
            services.AddScoped<IRefreshTokenRepository, SqlRefreshTokenRepository>();
            services.AddScoped<IPermissionRepository, SqlPermissionRepository>();
            services.AddScoped<IRoleRepository, SqlRoleRepository>();
            services.AddScoped<IRolePermissionRepository, SqlRolePermissionRepository>();

            // ========= ✅ Services =========
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<ILoginHistoryService, LoginHistoryService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IUserVerificationService, UserVerificationService>();
            services.AddScoped<IAuthService, AuthService>();

            // ========= ✅ Dependency Wrapper =========
            services.AddScoped<AuthServiceDependencies>();
            services.AddScoped<UserServiceDependencies>();

            // ========= ✅ Messaging (RabbitMQ) =========

            // 👇 Producer
            services.AddScoped<IRabbitMqPublisher, RabbitMqPublisherService>();
            services.AddScoped<IRabbitMqUserAuditMessageProducer, RabbitMqAuditProducer>();
            services.AddScoped<IRabbitMqLoginHistoryMessageProducer, RabbitMqLoginHistoryProducer>();
            services.AddScoped<IRabbitMqUserMessageProducer, RabbitMqUserMessageProducer>();

            // 👇 Consumer Handlers
            services.AddScoped<IRabbitMqUserAuditMessageHandler, RabbitMqUserAuditMessageHandler>();
            services.AddScoped<IRabbitMqLoginHistoryMessageHandler, RabbitMqLoginHistoryMessageHandler>();
            services.AddScoped<IRabbitUserRegisteredHandler, RabbitUserRegisteredHandler>();
            services.AddScoped<IRabbitUserUpdatedHandler, RabbitUserUpdatedHandler>();
            services.AddScoped<IRabbitUserDeletedHandler, RabbitUserDeletedHandler>();
            services.AddScoped<IRabbitUserPasswordChangedHandler, RabbitUserPasswordChangedHandler>();
            services.AddScoped<IRabbitEmailVerifiedHandler, RabbitEmailVerifiedHandler>();

            // 👇 Consumers (Singleton để giữ connection chạy suốt)
            services.AddScoped<IRabbitMqConsumer, RabbitMqUserAuditConsumer>();
            services.AddScoped<IRabbitMqConsumer, RabbitMqLoginHistoryConsumer>();
            services.AddScoped<IRabbitMqConsumer, RabbitUserRegisteredConsumer>();
            services.AddScoped<IRabbitMqConsumer, RabbitUserUpdatedConsumer>();
            services.AddScoped<IRabbitMqConsumer, RabbitUserDeletedConsumer>();
            services.AddScoped<IRabbitMqConsumer, RabbitUserPasswordChangedConsumer>();
            services.AddScoped<IRabbitMqConsumer, RabbitEmailVerifiedConsumer>();

            // ========= ✅ Redis =========
            services.AddScoped<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(config["Redis:ConnectionString"]!));

            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IRedisZSetService, RedisZSetService>();
            services.AddScoped<ILoginHistoryCacheService, LoginHistoryCacheService>();
            services.AddScoped<ILoginHistoryZSetTrackerService, LoginHistoryZSetTrackerService>();
            services.AddScoped<IUserAuditCacheService, UserAuditCacheService>();
            services.AddScoped<IUserCacheService, UserCacheService>();
            services.AddScoped<IAuditZSetTrackerService, AuditZSetTrackerService>();


            // ========= ✅ Email + Slack =========
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISlackService, SlackService>();

            // ========= ✅ Token, Identity =========
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICurrentUserContext, CurrentUserContext>();

            // ========= ✅ Seeder =========
            services.AddScoped<PermissionSeederService>();
            services.AddScoped<RoleSeederService>();
            services.AddScoped<RolePermissionSeederService>();
            services.AddScoped<UserRoleSeederService>();

            // ========= ✅ Hosted Services =========
            services.AddHostedService<VerificationTokenCleanupHostedService>();
            services.AddHostedService<RabbitMqConsumerService>();

            return services;
        }
    }
}


