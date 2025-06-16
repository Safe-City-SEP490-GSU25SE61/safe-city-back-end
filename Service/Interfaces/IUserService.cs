using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;

namespace Service.Interfaces;

public interface IUserService
{
    Task<UserProfileResponseModel?> GetUserByIdAsync(Guid id);
    Task<bool> UpdateUserAsync(Guid id, UpdateUserRequestModel model);
    public Task<bool> UpdateUserImageAsync(Guid id, string imageUrl);
}