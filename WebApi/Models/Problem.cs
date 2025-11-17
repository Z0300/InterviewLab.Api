using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Problem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    [Required, MaxLength(20)]
    public string Difficulty { get; set; } = "Medium";
    [MaxLength(300)]
    public string? TagsJson { get; set; }
    public List<Solution> Solutions { get; set; } = [];
}
