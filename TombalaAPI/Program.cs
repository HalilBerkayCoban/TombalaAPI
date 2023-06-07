using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using TombalaAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("TombalaDatabase"));

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

app.Run();
