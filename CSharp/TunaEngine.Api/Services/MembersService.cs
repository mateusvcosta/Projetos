using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using TunaEngine.Api.Interfaces;
using TunaEngine.Api.Models;

namespace TunaEngine.Api.Services;

public sealed class MembersService(
    IMongoCollection<Members> members,
    IPasswordHasher<Members> passwordHasher) : MembersInterface
{
    public async Task<IReadOnlyList<Members>> GetAllMembers(
        CancellationToken cancellationToken)
    {
        return await members
            .Find(FilterDefinition<Members>.Empty)
            .SortByDescending(member => member.DateAdded)
            .ToListAsync(cancellationToken);
    }

    public async Task<Members?> GetByIdMember(
        string id,
        CancellationToken cancellationToken)
    {
        return await members
            .Find(member => member.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Members?> Login(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim();
        var filter = Builders<Members>.Filter.Regex(
            member => member.Email,
            new BsonRegularExpression($"^{Regex.Escape(normalizedEmail)}$", "i"));
        var member = await members
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        if (member?.PasswordHash is null)
        {
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(
            member,
            member.PasswordHash,
            password);

        return verificationResult == PasswordVerificationResult.Failed ? null : member;
    }

    public async Task<Members> CreateMember(
        MembersRequest request,
        CancellationToken cancellationToken)
    {
        var id = ObjectId.GenerateNewId().ToString();
        var item = request.ToDocument(id, DateTime.UtcNow);
        item.PasswordHash = passwordHasher.HashPassword(item, request.Password!);
        await members.InsertOneAsync(item, cancellationToken: cancellationToken);
        return item;
    }

    public async Task<bool> UpdateMember(
        string id,
        MembersRequest request,
        CancellationToken cancellationToken)
    {
        var update = Builders<Members>.Update
            .Set(member => member.Name, request.Name!.Trim())
            .Set(member => member.Email, request.Email!.Trim())
            .Set(member => member.Phone, request.Phone?.Trim());

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var existingMember = await members
                .Find(member => member.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingMember is null)
            {
                return false;
            }

            update = update.Set(
                member => member.PasswordHash,
                passwordHasher.HashPassword(existingMember, request.Password.Trim()));
        }

        var result = await members.UpdateOneAsync(
            member => member.Id == id,
            update,
            cancellationToken: cancellationToken);

        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteMember(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await members.DeleteOneAsync(
            member => member.Id == id,
            cancellationToken);

        return result.DeletedCount > 0;
    }
}
