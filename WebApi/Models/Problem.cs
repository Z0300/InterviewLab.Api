using System.ComponentModel.DataAnnotations;
using WebApi.Models.Enums;

namespace WebApi.Models;

public class Problem
{
    public Guid Id { get; init; }

    [Required, MaxLength(100)] public required string Title { get; set; }
    [Required, MaxLength(100)] public required string Company { get; set; }
    [Required, MaxLength(4000)] public required string Description { get; set; } // markdown

    [Required, MaxLength(10)] public Difficulty Difficulty { get; set; } = Difficulty.Medium;

    [MaxLength(500)] public string[]? TagsJson { get; set; }

    public List<Solution> Solutions { get; init; } = [];
}