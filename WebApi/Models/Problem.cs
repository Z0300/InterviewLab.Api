using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Problem
{
    public Guid Id { get; init; }

    [Required, MaxLength(100)] public required string Title { get; set; }
    [Required, MaxLength(5000)] public required string Description { get; set; } // markdown

    [Required, MaxLength(20)] public string Difficulty { get; set; } = "Medium";

    [MaxLength(300)] public string? TagsJson { get; set; }

    public List<Solution> Solutions { get; init; } = [];
}