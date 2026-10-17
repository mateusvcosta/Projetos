namespace TunaEngine.Api.Models;

public sealed class MembersRequest
{
    public string? Name { get; init; }
    public string? TunaName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? StudentNumber { get; init; }
    public string? RoleId { get; init; }
    public string? VoiceType { get; init; }
    public string? Instrument { get; init; }
    public string? Notes { get; init; }

    public Dictionary<string, string[]> GetValidationErrors()
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Name))
        {
            errors[nameof(Name)] = ["Name is required."];
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            errors[nameof(Email)] = ["Email is required."];
        }

        return errors;
    }

    public Members ToDocument(string id, DateTime dateAdded)
    {
        var memberId = $"MEM-{id}";

        return new Members
        {
            Id = id,
            MemberId = memberId,
            Name = Name!.Trim(),
            TunaName = TunaName?.Trim() ?? string.Empty,
            Email = Email!.Trim(),
            Phone = Phone?.Trim(),
            StudentNumber = StudentNumber?.Trim(),
            RoleId = RoleId?.Trim(),
            VoiceType = VoiceType?.Trim(),
            Instrument = Instrument?.Trim(),
            Notes = Notes?.Trim()
        };
    }
}
