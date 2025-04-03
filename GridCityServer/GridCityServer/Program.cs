using GridCityServer.Database;
using GridCityServer.Hubs;
using GridCityServer.Infrastructure;
using GridCityServer.Infrastructure.AuthSecurity;
using GridCityServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("default")));

var redisUrl = configuration["Redis:Url"] ?? throw new InvalidOperationException("Redis connection not set");
var redis = await ConnectionMultiplexer.ConnectAsync(redisUrl);
if (redis == null) throw new InvalidOperationException("Could not connect to Redis");

builder.Services.AddLogging(config => config.AddConsole());

builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<ILobbiesRepository, RedisLobbiesRepository>();
builder.Services.AddScoped<ILobbiesService, LobbiesService>();
builder.Services.AddScoped<IGameSessionRepository, RedisGameSessionRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IGameplayService, GameplayService>();
builder.Services.AddScoped<IMatchHistoryService, MatchHistoryService>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = configuration[$"{JwtGenerator.ConfigurationSection}:Issuer"];
        options.Audience = configuration[$"{JwtGenerator.ConfigurationSection}:Audience"];
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[$"{JwtGenerator.ConfigurationSection}:SecretKey"])),

            ValidateIssuer = true,
            ValidIssuer = configuration[$"{JwtGenerator.ConfigurationSection}:Issuer"],

            ValidateAudience = true,
            ValidAudience = configuration[$"{JwtGenerator.ConfigurationSection}:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/lobbies")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    var request = context.Request;
    var authorizationHeader = request.Headers["Authorization"].ToString();
    var queryParams = request.QueryString.HasValue ? request.QueryString.Value : "No query params";

    Console.WriteLine($"[{DateTime.UtcNow}] Request: {request.Protocol} {request.Method} {request.Path} {queryParams}");

    if (!string.IsNullOrEmpty(authorizationHeader))
    {
        Console.WriteLine($"Authorization: {authorizationHeader}");
    }

    await next();

    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<LobbyHub>("/lobbies");
app.MapHub<GameplayHub>("/gameplay");

app.Run();
