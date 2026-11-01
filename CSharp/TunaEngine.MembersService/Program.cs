using MongoDB.Driver;
using TunaEngine.MembersService.Interfaces;
using TunaEngine.MembersService.Models;
using TunaEngine.MembersService.Services;
using TunaEngine.MembersService.Settings;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var mongoSettings = builder.Configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>()
    ?? throw new InvalidOperationException("MongoDB configuration is missing.");
if (string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
    throw new InvalidOperationException("MongoDB connection string is missing. Set MongoDb__ConnectionString.");
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.RolesDatabaseName).GetCollection<Roles>(mongoSettings.RolesCollectionName));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.MembersDatabaseName).GetCollection<Members>(mongoSettings.MembersCollectionName));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.MembersDatabaseName).GetCollection<MongoDB.Bson.BsonDocument>("Sequences"));
builder.Services.AddSingleton<SequenceGenerator>();
builder.Services.AddScoped<IPasswordHasher<Members>, PasswordHasher<Members>>();
builder.Services.AddScoped<MembersInterface, MembersService>();
builder.Services.AddScoped<RolesInterface, RolesService>();
builder.Services.AddControllers();
var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "TunaEngine.MembersService" }));
app.MapControllers();
app.Run();
