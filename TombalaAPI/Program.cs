using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using TombalaAPI.Hubs;
using TombalaAPI.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Env.Load(); 
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Configuration.AddEnvironmentVariables();

// Configure PostgreSQL with EF Core
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication("cookie").AddCookie("cookie").AddOAuth("Discord", options =>
{
    options.SignInScheme = "cookie";
    options.ClientId = builder.Configuration.GetValue<string>("Discord:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("Discord:ClientSecret");
    options.AuthorizationEndpoint = "https://discord.com/oauth2/authorize";
    options.TokenEndpoint = "https://discord.com/api/oauth2/token";
    options.CallbackPath = "/oauth/discord-cb";
    options.Scope.Add("identify");
    options.SaveTokens = true;
    options.UserInformationEndpoint = "https://discord.com/api/users/@me";
    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");

    options.Events.OnCreatingTicket = async ctx =>
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);

        var response = await ctx.Backchannel.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<JsonElement>();
        ctx.RunClaimActions(user);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply migrations at startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
}

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/login", () =>
{
    return Results.Challenge(new AuthenticationProperties
    {
        RedirectUri = "your-redirect-uri"
    }, authenticationSchemes: new List<string>() { "Discord" });
});

app.MapControllers();
app.MapHub<GameHub>("/gamehub");

app.Run();
