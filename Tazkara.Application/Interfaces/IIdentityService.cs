using Tazkara.Application.DTOs.Auth;
using Tazkara.Application.Wrappers;

namespace Tazkara.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    }
}
