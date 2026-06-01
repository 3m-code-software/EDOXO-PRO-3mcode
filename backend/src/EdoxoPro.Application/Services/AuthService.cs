using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Auth;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EdoxoPro.Application.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<RefreshToken> _refreshTokenRepo;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IGenericRepository<User> userRepo,
        IGenericRepository<RefreshToken> refreshTokenRepo,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userRepo = userRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = (await _userRepo.FindAsync(u =>
                (u.Username == request.Username || u.Email == request.Username) && !u.IsDeleted)).FirstOrDefault();

            if (user == null)
                return ApiResponse<LoginResponse>.Fail("اسم المستخدم أو كلمة المرور غير صحيحة");

            if (!user.IsActive)
                return ApiResponse<LoginResponse>.Fail("الحساب غير نشط، يرجى الاتصال بالمسؤول");

            var passwordHash = HashPassword(request.Password);
            if (user.PasswordHash != passwordHash)
                return ApiResponse<LoginResponse>.Fail("اسم المستخدم أو كلمة المرور غير صحيحة");

            var roles = user.Roles?.Select(r => r.Role.Name).ToList() ?? new List<string>();
            var (token, expiresAt) = GenerateJwtToken(user, roles);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            user.LastLoginAt = DateTime.UtcNow;
            _userRepo.Update(user);

            var response = new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                ExpiresAt = expiresAt,
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    FullNameAr = user.FullNameAr,
                    Roles = roles
                }
            };

            return ApiResponse<LoginResponse>.Ok(response, "تم تسجيل الدخول بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تسجيل الدخول للمستخدم {Username}", request.Username);
            return ApiResponse<LoginResponse>.Fail("حدث خطأ أثناء تسجيل الدخول");
        }
    }

    public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var existingUser = (await _userRepo.FindAsync(u =>
                u.Username == request.Username || u.Email == request.Email)).FirstOrDefault();

            if (existingUser != null)
                return ApiResponse<string>.Fail("اسم المستخدم أو البريد الإلكتروني موجود بالفعل");

            var user = _mapper.Map<User>(request);
            user.PasswordHash = HashPassword(request.Password);
            user.CreatedAt = DateTime.UtcNow;

            await _userRepo.AddAsync(user);

            return ApiResponse<string>.Ok(string.Empty, "تم تسجيل المستخدم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تسجيل مستخدم جديد");
            return ApiResponse<string>.Fail("حدث خطأ أثناء تسجيل المستخدم");
        }
    }

    public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var token = (await _refreshTokenRepo.FindAsync(r =>
                r.Token == request.RefreshToken && !r.IsRevoked)).FirstOrDefault();

            if (token == null)
                return ApiResponse<LoginResponse>.Fail("رمز التحديث غير صالح");

            if (token.ExpiresAt < DateTime.UtcNow)
                return ApiResponse<LoginResponse>.Fail("انتهت صلاحية رمز التحديث");

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            _refreshTokenRepo.Update(token);

            var user = await _userRepo.GetByIdAsync(token.UserId);
            if (user == null || !user.IsActive)
                return ApiResponse<LoginResponse>.Fail("المستخدم غير موجود أو غير نشط");

            var roles = user.Roles?.Select(r => r.Role.Name).ToList() ?? new List<string>();
            var (jwt, expiresAt) = GenerateJwtToken(user, roles);
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);

            var response = new LoginResponse
            {
                Token = jwt,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = expiresAt,
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    FullNameAr = user.FullNameAr,
                    Roles = roles
                }
            };

            return ApiResponse<LoginResponse>.Ok(response, "تم تحديث الرمز بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث رمز الدخول");
            return ApiResponse<LoginResponse>.Fail("حدث خطأ أثناء تحديث الرمز");
        }
    }

    public async Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail("المستخدم غير موجود");

            var oldHash = HashPassword(request.OldPassword);
            if (user.PasswordHash != oldHash)
                return ApiResponse<string>.Fail("كلمة المرور القديمة غير صحيحة");

            user.PasswordHash = HashPassword(request.NewPassword);
            _userRepo.Update(user);

            return ApiResponse<string>.Ok(string.Empty, "تم تغيير كلمة المرور بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تغيير كلمة المرور للمستخدم {UserId}", userId);
            return ApiResponse<string>.Fail("حدث خطأ أثناء تغيير كلمة المرور");
        }
    }

    public async Task<ApiResponse<UserInfo>> GetProfileAsync(int userId)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserInfo>.Fail("المستخدم غير موجود");

            var userInfo = _mapper.Map<UserInfo>(user);
            userInfo.Roles = user.Roles?.Select(r => r.Role.Name).ToList() ?? new List<string>();

            return ApiResponse<UserInfo>.Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الملف الشخصي للمستخدم {UserId}", userId);
            return ApiResponse<UserInfo>.Fail("حدث خطأ أثناء جلب الملف الشخصي");
        }
    }

    public async Task<ApiResponse<UserInfo>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserInfo>.Fail("المستخدم غير موجود");

            if (request.FullName != null) user.FullName = request.FullName;
            if (request.FullNameAr != null) user.FullNameAr = request.FullNameAr;
            if (request.Email != null) user.Email = request.Email;
            if (request.Phone != null) user.Phone = request.Phone;
            user.UpdatedAt = DateTime.UtcNow;
            _userRepo.Update(user);

            var userInfo = _mapper.Map<UserInfo>(user);
            userInfo.Roles = user.Roles?.Select(r => r.Role.Name).ToList() ?? new List<string>();

            return ApiResponse<UserInfo>.Ok(userInfo, "تم تحديث الملف الشخصي بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث الملف الشخصي للمستخدم {UserId}", userId);
            return ApiResponse<UserInfo>.Fail("حدث خطأ أثناء تحديث الملف الشخصي");
        }
    }

    private (string token, DateTime expiresAt) GenerateJwtToken(User user, List<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "EdoxoProSecretKey2024!@#$%^&*()SuperSecretKey"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (roles.Count != 0)
            claims.Add(new Claim("roles", string.Join(",", roles)));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "EdoxoPro",
            audience: _configuration["Jwt:Audience"] ?? "EdoxoPro",
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(int userId)
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(randomBytes),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepo.AddAsync(refreshToken);
        return refreshToken;
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
