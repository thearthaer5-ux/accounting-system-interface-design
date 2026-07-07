using AutoMapper;
using EFA.Application.DTOs;
using EFA.Domain.Entities;
using EFA.Infrastructure.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace EFA.Application.Services;

/// <summary>
/// تطبيق خدمات المستخدمين
/// يوفر عمليات إدارة المستخدمين والمصادقة
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPrivilegeRepository _privilegeRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IPrivilegeRepository privilegeRepository,
        IAuditRepository auditRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _privilegeRepository = privilegeRepository;
        _auditRepository = auditRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDto<UserDto>> RegisterAsync(CreateUserDto createUserDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
            if (existingUser != null)
                return new ResponseDto<UserDto> { Success = false, Message = "اسم المستخدم موجود بالفعل" };

            var existingEmail = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingEmail != null)
                return new ResponseDto<UserDto> { Success = false, Message = "البريد الإلكتروني موجود بالفعل" };

            // Create new user
            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                FullName = createUserDto.FullName,
                PhoneNumber = createUserDto.PhoneNumber,
                PasswordHash = HashPassword(createUserDto.Password),
                GroupId = createUserDto.GroupId,
                BranchId = createUserDto.BranchId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Log audit
            await LogAuditAsync(null, "User", "Create", user.UserId, null, createUserDto.Username);

            var userDto = _mapper.Map<UserDto>(user);
            return new ResponseDto<UserDto> { Success = true, Message = "تم إنشاء المستخدم بنجاح", Data = userDto };
        }
        catch (Exception ex)
        {
            return new ResponseDto<UserDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء إنشاء المستخدم",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<UserDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return new ResponseDto<UserDto> { Success = false, Message = "بيانات الدخول غير صحيحة" };

            if (!user.IsActive)
                return new ResponseDto<UserDto> { Success = false, Message = "حساب المستخدم معطل" };

            // Update last login
            await _userRepository.UpdateLastLoginAsync(user.UserId);

            // Log audit
            await LogAuditAsync(user.UserId, "User", "Login", user.UserId, null, "دخول المستخدم");

            var userDto = _mapper.Map<UserDto>(user);
            return new ResponseDto<UserDto> { Success = true, Message = "تم تسجيل الدخول بنجاح", Data = userDto };
        }
        catch (Exception ex)
        {
            return new ResponseDto<UserDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تسجيل الدخول",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(changePasswordDto.UserId);
            if (user == null)
                return new ResponseDto<bool> { Success = false, Message = "المستخدم غير موجود" };

            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                return new ResponseDto<bool> { Success = false, Message = "كلمة المرور الحالية غير صحيحة" };

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                return new ResponseDto<bool> { Success = false, Message = "كلمات المرور الجديدة غير متطابقة" };

            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            user.LastModifiedDate = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            await LogAuditAsync(user.UserId, "User", "ChangePassword", user.UserId, null, "تغيير كلمة المرور");

            return new ResponseDto<bool> { Success = true, Message = "تم تغيير كلمة المرور بنجاح", Data = true };
        }
        catch (Exception ex)
        {
            return new ResponseDto<bool> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تغيير كلمة المرور",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetUserWithGroupAsync(userId);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<PaginatedResponseDto<UserDto>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10)
    {
        var (users, total) = await _userRepository.GetPagedAsync(pageNumber, pageSize, null, q => q.OrderByDescending(u => u.CreatedDate));
        
        return new PaginatedResponseDto<UserDto>
        {
            Items = _mapper.Map<List<UserDto>>(users),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ResponseDto<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(updateUserDto.UserId);
            if (user == null)
                return new ResponseDto<UserDto> { Success = false, Message = "المستخدم غير موجود" };

            user.Email = updateUserDto.Email;
            user.FullName = updateUserDto.FullName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.IsActive = updateUserDto.IsActive;
            user.GroupId = updateUserDto.GroupId;
            user.BranchId = updateUserDto.BranchId;
            user.LastModifiedDate = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            await LogAuditAsync(null, "User", "Update", user.UserId, null, $"تحديث بيانات المستخدم {user.Username}");

            var userDto = _mapper.Map<UserDto>(user);
            return new ResponseDto<UserDto> { Success = true, Message = "تم تحديث المستخدم بنجاح", Data = userDto };
        }
        catch (Exception ex)
        {
            return new ResponseDto<UserDto> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تحديث المستخدم",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<bool>> DeactivateUserAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new ResponseDto<bool> { Success = false, Message = "المستخدم غير موجود" };

            user.IsActive = false;
            user.LastModifiedDate = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            await LogAuditAsync(null, "User", "Deactivate", user.UserId, null, $"تعطيل حساب المستخدم {user.Username}");

            return new ResponseDto<bool> { Success = true, Message = "تم تعطيل المستخدم بنجاح", Data = true };
        }
        catch (Exception ex)
        {
            return new ResponseDto<bool> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تعطيل المستخدم",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResponseDto<bool>> ActivateUserAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new ResponseDto<bool> { Success = false, Message = "المستخدم غير موجود" };

            user.IsActive = true;
            user.LastModifiedDate = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            await LogAuditAsync(null, "User", "Activate", user.UserId, null, $"تفعيل حساب المستخدم {user.Username}");

            return new ResponseDto<bool> { Success = true, Message = "تم تفعيل المستخدم بنجاح", Data = true };
        }
        catch (Exception ex)
        {
            return new ResponseDto<bool> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تفعيل المستخدم",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<List<PrivilegeDto>> GetUserPrivilegesAsync(int userId)
    {
        var privileges = await _privilegeRepository.GetUserPrivilegesAsync(userId);
        return _mapper.Map<List<PrivilegeDto>>(privileges);
    }

    public async Task<bool> HasPrivilegeAsync(int userId, string privilegeCode)
    {
        return await _privilegeRepository.HasPrivilegeAsync(userId, privilegeCode);
    }

    public async Task<ResponseDto<bool>> AssignGroupToUserAsync(int userId, int groupId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new ResponseDto<bool> { Success = false, Message = "المستخدم غير موجود" };

            user.GroupId = groupId;
            user.LastModifiedDate = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            await LogAuditAsync(null, "User", "AssignGroup", user.UserId, null, $"تعيين مجموعة للمستخدم {user.Username}");

            return new ResponseDto<bool> { Success = true, Message = "تم تعيين المجموعة بنجاح", Data = true };
        }
        catch (Exception ex)
        {
            return new ResponseDto<bool> 
            { 
                Success = false, 
                Message = "حدث خطأ أثناء تعيين المجموعة",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    // Helper methods
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput.Equals(hash);
    }

    private async Task LogAuditAsync(int? userId, string entityName, string action, int? entityId, string? oldValue, string? newValue)
    {
        var audit = new Audit
        {
            UserId = userId,
            EntityName = entityName,
            Action = action,
            EntityId = entityId,
            OldValues = oldValue,
            NewValues = newValue,
            AuditDate = DateTime.UtcNow,
            IsSuccessful = true
        };

        await _auditRepository.AddAsync(audit);
        await _auditRepository.SaveChangesAsync();
    }
}
