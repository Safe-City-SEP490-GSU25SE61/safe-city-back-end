namespace BusinessObject.DTOs.RequestModels;

public class UpdateUserRequestModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public bool Gender { get; set; }
    public string Phone { get; set; } = string.Empty;
}