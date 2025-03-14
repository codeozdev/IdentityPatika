using System.ComponentModel.DataAnnotations;

namespace IdentityPatika.ViewModel;

public class LoginViewModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}