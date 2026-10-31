using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TunaEngine.Auth.Api.Models;

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

if (string.IsNullOrWhiteSpace(jwtSecret) || System.Text.Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException("Jwt:Secret must contain at least 32 bytes.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience are required.");
}

var client = new MongoClient(mongoConnection);
var users = client.GetDatabase(databaseName).GetCollection<AuthUser>("users");
await users.Indexes.CreateOneAsync(new CreateIndexModel<AuthUser>(
    Builders<AuthUser>.IndexKeys.Ascending(user => user.Email),
    new CreateIndexOptions { Unique = true }));

builder.Services.AddSingleton(users);
builder.Services.AddSingleton<IPasswordHasher<AuthUser>, PasswordHasher<AuthUser>>();
builder.Services.AddSingleton(new JwtSettings(jwtSecret, jwtIssuer, jwtAudience));
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "auth" }));
app.MapControllers();
app.Run();

public sealed record JwtSettings(string Secret, string Issuer, string Audience);
