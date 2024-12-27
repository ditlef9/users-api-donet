using Newtonsoft.Json;
namespace UsersApiDotnet.Dtos;
public partial class UserForRegistrationDto
{
    [JsonProperty("Email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("Password")]
    public string Password { get; set; } = string.Empty;

    [JsonProperty("PasswordConfirm")]
    public string PasswordConfirm { get; set; } = string.Empty;

    [JsonProperty("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("LastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("Gender")]
    public string Gender { get; set; } = string.Empty;
}
