using System.ComponentModel.DataAnnotations;

namespace IdentityPatika.ViewModel;

public class RegisterViewModel
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}