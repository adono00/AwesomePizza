using System.ComponentModel.DataAnnotations;

namespace AwesomePizza.Service;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required, MinLength(32)]
    public string Key { get; set; } = "";

    [Required]
    public string Issuer { get; set; } = "";

    [Required]
    public string Audience { get; set; } = "";

    [Range(1, 1440)]
    public int ExpiresMinutes { get; set; } = 30;
}