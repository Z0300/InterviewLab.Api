using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Solution
{
    public Guid Id { get; init; }

    [Required] public required Guid ProblemId { get; init; }
    public Problem? Problem { get; init; }

    [Required, MaxLength(30)] public string Language { get; set; } = "csharp";

    [Required, MaxLength(4000)] public required string Code { get; set; }
    [MaxLength(4000)] public string? Explanation { get; set; } // markdown

    public bool IsCanonical { get; set; }
    public int QualityScore { get; set; }

    [MaxLength(100)] public string? Source { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}