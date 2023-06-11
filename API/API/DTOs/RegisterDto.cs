using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required] public string Username { get; set; }
    [Required] public string KnownAs { get; set; }
    [Required] public string Gender { get; set; } = "";
    public DateOnly DateOfBirth { get; set; }
    [Required] public string City { get; set; }
    [Required] public string Country { get; set; }
    public string? Interests { get; set; }
    public string? Introduction { get; set; }
    public string? LookingFor { get; set; }

    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; }
}