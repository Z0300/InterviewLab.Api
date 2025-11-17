using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class User
{
    public Guid Id { get; set; }
    [Required, MaxLength(20)]
    public required string Username { get; set; }
    [Required, MaxLength(512)]
    public required string PasswordHash { get; set; }
}
