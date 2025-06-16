using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.RequestModels;

public class UserRegistrationRequestModel
{
    [JsonPropertyName("fullName")]
    [Required(ErrorMessage = "Full name is mandatory")]
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters long")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    [Required(ErrorMessage = "Email cannot be blank")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    [Required(ErrorMessage = "Password cannot be blank")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,16}$",
        ErrorMessage = "Minimum 8 characters, at least one uppercase letter and one number")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("dateOfBirth")]
    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [JsonPropertyName("phone")]
    [Required(ErrorMessage = "Phone cannot be blank")]
    [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b",
        ErrorMessage = "Please enter a valid (+84) phone number")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    [Required(ErrorMessage = "Gender is required")]
    public bool Gender { get; set; }
}