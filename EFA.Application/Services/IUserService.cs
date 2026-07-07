using EFA.Application.DTOs;

namespace EFA.Application.Services;

/// <summary>
/// واجهة خدمات المستخدمين
/// توفر عمليات إدارة المستخدمين والمصادقة
/// </summary>
public interface IUserService
{
    Task<ResponseDto<UserDto>> RegisterAsync(CreateUserDto createUserDto);
    Task<ResponseDto<UserDto>> LoginAsync(LoginDto loginDto);
    Task<ResponseDto<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<PaginatedResponseDto<UserDto>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10);
    Task<ResponseDto<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto);
    Task<ResponseDto<bool>> DeactivateUserAsync(int userId);
    Task<ResponseDto<bool>> ActivateUserAsync(int userId);
    Task<List<PrivilegeDto>> GetUserPrivilegesAsync(int userId);
    Task<bool> HasPrivilegeAsync(int userId, string privilegeCode);
    Task<ResponseDto<bool>> AssignGroupToUserAsync(int userId, int groupId);
}
