using System.ComponentModel.DataAnnotations;

namespace TunaEngine.Members.Api.Models;

public sealed class MemberProfileRequest
{
    [Required, StringLength(120)]
    public string? TunaName { get; init; }

    [Phone, StringLength(40)]
    public string? Phone { get; init; }

    [StringLength(40)]
    public string? StudentNumber { get; init; }

    [StringLength(80)]
    public string? VoiceType { get; init; }

    [StringLength(80)]
    public string? Instrument { get; init; }
}
