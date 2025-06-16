namespace BusinessObject.DTOs.ResponseModels;

public class UserProfileResponseModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public bool Gender { get; set; }
    public string Phone { get; set; }
    public string Status { get; set; }
    public bool IsLoggedIn { get; set; }
}