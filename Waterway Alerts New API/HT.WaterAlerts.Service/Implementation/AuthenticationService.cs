using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HT.WaterAlerts.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace HT.WaterAlerts.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private double _tokenExpiry;
        private readonly ILogger<AuthenticationService> _logger;
        private const string userType= "public";
        public AuthenticationService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenExpiry = Convert.ToDouble(_configuration.GetSection("TokenExpireIn").Value.ToString());
            _logger = logger;
        }

        public async Task<IdentityResult> SignUp(SignUpDTO signUpUser)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var appUser = new ApplicationUser()
                    {
                        Email = signUpUser.Email,
                        FirstName = signUpUser.FirstName,
                        LastName = signUpUser.LastName,
                        PhoneNumber = signUpUser.MobileNumber,
                        UserName = signUpUser.Email,
                        Type = userType,
                        CreatedDate = DateTime.Now,
                        Status = true
                    };
                    IdentityResult result = await _userManager.CreateAsync(appUser, signUpUser.Password);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddToRoleAsync(appUser, "customer");
                        if (result.Succeeded)
                        {
                            scope.Complete();
                            return result;
                        }
                    }
                    _logger.LogError("SingUp failed for user " + signUpUser.Email, result.Errors);
                    scope.Dispose();
                    return result;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    _logger.LogError("SingUp error for email " + signUpUser.Email, ex.Message);
                    throw;
                }
            }
        }

        public async Task<SignInResponseDTO> SignIn(string email, string password)
        {
            try
            {
                if (!(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)))
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        if (await _userManager.IsEmailConfirmedAsync(user) && user.Status)
                        {
                            if (await _userManager.CheckPasswordAsync(user, password))
                            {
                                IList<string> userRoles = await _userManager.GetRolesAsync(user);
                                if (userRoles.Count > 0)
                                {
                                    return new SignInResponseDTO()
                                    {
                                        Email = user.Email,
                                        Id = user.Id,
                                        FirstName = user.FirstName,
                                        LastName = user.LastName,
                                        Type = user.Type,
                                        Token = GenerateJwtToken(user, userRoles),
                                        ExpireIn = _tokenExpiry.ToString(),
                                        SkipVideo = user.SkipVideo,
                                        SkipTutorial = user.SkipTutorial,
                                        SkipTermsAndConditions = user.SkipTermsAndConditions,
                                        Roles = userRoles.ToList()
                                    };
                                }
                                else
                                {
                                    _logger.LogError("User has no roles for email " + email);
                                    throw new Exception("User has no roles");
                                }
                            }
                            else
                            {
                                _logger.LogError("Password does not match for email " + email);
                                throw new Exception("Password does not match");
                            }
                        }
                        else
                        {
                            _logger.LogError("User is not active for email " + email);
                            throw new Exception("User has not active");
                        }
                    }
                    else
                    {
                        _logger.LogError("User not found with email " + email);
                        throw new Exception("User not found");
                    }
                }
                else
                {
                    _logger.LogError("Email or Password is empty");
                    throw new ArgumentNullException("Email or Password is empty");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SignIn error for email " + email, ex.Message);
                throw new Exception("Invalid Email or Password");
            }
        }

        public async Task<SignInResponseDTO> AutoSignIn(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                IList<string> userRoles = await _userManager.GetRolesAsync(user);
                if(!user.Status)
                {
                    _logger.LogError("Auto login error: User is not active for email " + email);
                    throw new Exception("Invalid Email or Password");
                }
                if (userRoles.Count > 0)
                {
                    return new SignInResponseDTO()
                    {
                        Email = user.Email,
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Type = user.Type,
                        Token = GenerateJwtToken(user, userRoles),
                        ExpireIn = _tokenExpiry.ToString(),
                        SkipVideo = user.SkipVideo,
                        SkipTutorial = user.SkipTutorial,
                        SkipTermsAndConditions = user.SkipTermsAndConditions,
                        Roles = userRoles.ToList()
                    };
                }
                else
                {
                    _logger.LogError("Auto login error: User has no roles for email " + email);
                    throw new Exception("Invalid Email or Password");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Auto login error for email " + email, ex.Message);
                throw;
            }
        }

        public async Task<string> GenerateEmailToken(string email, string type)
        {
            try
            {
                ApplicationUser appUser = await _userManager.FindByEmailAsync(email);
                if (appUser == null)
                {
                    throw new Exception("Token generation error: Email not found for " + email);
                }

                if (type == "confirm")
                {
                    return await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                }
                else
                {
                    return await _userManager.GeneratePasswordResetTokenAsync(appUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Token generation error " + email.ToString(), ex.Message);
                throw new Exception("Error Occured"); ;
            }
        }

        public async Task<IdentityResult> ConfirmEmail(string email, string token)
        {
            try
            {
                ApplicationUser appUser = await _userManager.FindByEmailAsync(email);
                IdentityResult result = await _userManager.ConfirmEmailAsync(appUser, token);

                if (!result.Succeeded)
                    _logger.LogError("Confirm email error " + email, result.Errors);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Confirm email error " + email, ex.Message);
                throw;
            }
        }

        public async Task<IdentityResult> ResetPassword(string email, string token, string newPassword)
        {
            try
            {
                ApplicationUser appUser = await _userManager.FindByEmailAsync(email);
                IdentityResult result = await _userManager.ResetPasswordAsync(appUser, token, newPassword);
                
                if(!result.Succeeded)
                    _logger.LogError("Reset password error " + email, result.Errors);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Reset password error " + email, ex.Message);
                throw;
            }
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("SecuredKey").Value.ToString());
            ClaimsIdentity claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));
            foreach (string role in roles)
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddSeconds(_tokenExpiry),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}