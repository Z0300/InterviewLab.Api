using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Solution
{
    public Guid Id { get; set; }

    [Required]
    public Guid ProblemId { get; set; }
    public Problem Problem { get; set; } = default!;

    [Required, MaxLength(30)]
    public string Language { get; set; } = "csharp";

    [Required]
    public string Code { get; set; } = default!;

    public string Explanation { get; set; } = "";
    public bool IsCanonical { get; set; } = false;
    public int QualityScore { get; set; } = 0;

    [MaxLength(100)]
    public string Source { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
