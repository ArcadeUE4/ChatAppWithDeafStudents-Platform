using ChatAppWithDeafStudents.API;
using ChatAppWithDeafStudents.API.Configuration;
using ChatAppWithDeafStudents.API.Controllers;
using ChatAppWithDeafStudents.API.Functions.Chat;
using ChatAppWithDeafStudents.API.Functions.Message;
using ChatAppWithDeafStudents.API.Functions.User;
using ChatAppWithDeafStudents.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddDebug();
    config.AddConsole();
#if DEBUG
    config.SetMinimumLevel(LogLevel.Debug);
#else
    config.SetMinimumLevel(LogLevel.Information);
#endif
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT key not found in configuration.");
}


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5) 
        };

        // Support token in SignalR connections
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Check for Authorization header first
                var authHeader = context.Request.Headers["Authorization"].ToString();

                // For SignalR, also check query string token
                if (string.IsNullOrEmpty(authHeader) && context.Request.Query.TryGetValue("access_token", out var queryToken))
                {
                    context.Token = queryToken.ToString();
                }
                else if (!string.IsNullOrEmpty(authHeader) 
                && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var bearerToken = authHeader.Substring(7); 
                    context.Token = bearerToken;
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Token validation succeeded
                return Task.CompletedTask;
            }
        };
    });


var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(",", StringSplitOptions.RemoveEmptyEntries) 
    ?? new[] { "https://localhost:7000", "https://localhost:5000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiConstants.CorsPolicy.AllowSpecific, corsBuilder =>
    {
        corsBuilder.WithOrigins(allowedOrigins)
                   .AllowAnyMethod()
                   .WithHeaders("Content-Type", "Authorization")
                   .AllowCredentials()
                   .WithExposedHeaders("Content-Type");
    });
});

builder.Services.AddTransient<IUserFunction, UserFunction>();
builder.Services.AddTransient<IChatFunction, ChatFunction>();
builder.Services.AddTransient<IMessageFunction, MessageFunction>();
builder.Services.AddSingleton<ITokenService, TokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors(ApiConstants.CorsPolicy.AllowSpecific);


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>(ApiConstants.SignalR.HubRoute);
app.Run();
