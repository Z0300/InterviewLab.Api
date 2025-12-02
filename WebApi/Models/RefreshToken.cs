using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class RefreshToken
{
    public Guid Id { get; init; }
    [Required, MaxLength(512)] public required string Token { get; set; }
    [Required, MaxLength(100)] public required Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }

    public User? User { get; set; }
}