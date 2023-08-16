using Microsoft.AspNetCore.Identity;
using HT.WaterAlerts.Domain;

namespace HT.WaterAlerts.Service
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> SignUp(SignUpDTO singUpUser);
        Task<SignInResponseDTO> SignIn(string email, string password);
        Task<SignInResponseDTO> AutoSignIn(string email);
        Task<string> GenerateEmailToken(string email, string type);
        Task<IdentityResult> ConfirmEmail(string email, string token);
        Task<IdentityResult> ResetPassword(string email, string token, string newPassword);
    }
}
