using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using TunaEngine.Members.Api.Models;
using TunaEngine.Members.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var mongoConnection = builder.Configuration["MongoDb:ConnectionString"];
var databaseName = builder.Configuration["MongoDb:DatabaseName"];
var jwtSecret = builder.Configuration["Jwt:Secret"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(mongoConnection) || string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException("MongoDb:ConnectionString and MongoDb:DatabaseName are required.");
}

if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException("Jwt:Secret must contain at least 32 bytes.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience are required.");
}

var client = new MongoClient(mongoConnection);
var profiles = client.GetDatabase(databaseName).GetCollection<MemberProfile>("members");
builder.Services.AddSingleton(profiles);
builder.Services.AddScoped<MemberProfileService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "members" }));
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
