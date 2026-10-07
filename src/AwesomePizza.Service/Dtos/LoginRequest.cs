using System.ComponentModel.DataAnnotations;

namespace AwesomePizza.Service.Dtos;

public class LoginRequest
{
    [Required]
    public string UserName { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}