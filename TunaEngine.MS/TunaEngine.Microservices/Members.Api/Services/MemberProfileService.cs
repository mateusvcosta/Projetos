using MongoDB.Driver;
using TunaEngine.Members.Api.Models;

namespace TunaEngine.Members.Api.Services;

public sealed class MemberProfileService(IMongoCollection<MemberProfile> profiles)
{
    public async Task<MemberProfile?> GetById(string id, CancellationToken cancellationToken) =>
        await profiles.Find(profile => profile.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<MemberProfile> Upsert(
        string id,
        string name,
        string email,
        MemberProfileRequest request,
        CancellationToken cancellationToken)
    {
        var profile = new MemberProfile
        {
            Id = id,
            Name = name,
            Email = email,
            TunaName = request.TunaName!.Trim(),
            Phone = request.Phone?.Trim(),
            StudentNumber = request.StudentNumber?.Trim(),
            VoiceType = request.VoiceType?.Trim(),
            Instrument = request.Instrument?.Trim()
        };

        await profiles.ReplaceOneAsync(
            current => current.Id == id,
            profile,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
        return profile;
    }
}
