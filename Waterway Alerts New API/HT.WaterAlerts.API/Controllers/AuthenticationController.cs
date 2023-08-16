using Microsoft.AspNetCore.Mvc;
using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using HT.WaterAlerts.Common.Email;
using System.Transactions;
using Microsoft.Extensions.Configuration;

namespace HT.WaterAlerts.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationService _authenticationService;
        private readonly ISmtpClient _smtpClient;

        public AuthenticationController(IAuthenticationService authenticationService, ISmtpClient smtpClient, IConfiguration configuration)
        {
            _authenticationService = authenticationService;
            _smtpClient = smtpClient;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Signin([FromBody] SignInDTO signInUser)
        {
            try
            {
                var result = await _authenticationService.SignIn(signInUser.Email, signInUser.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO signUpUser)
        {
            try
            {
                var result = await _authenticationService.SignUp(signUpUser);
                if (result.Succeeded)
                {
                    await ProcessEmail(signUpUser.Email, "confirm");
                    return Ok();
                }
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = result.Errors });
            }
            catch (Exception ex)
            {
                if (ex.Source == "System.Net.Mail")
                {
                    return Ok();
                }
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDTO confirmEmail)
        {
            try
            {
                var result = await _authenticationService.ConfirmEmail(confirmEmail.Email, confirmEmail.Token);
                if (result.Succeeded)
                {
                    return Ok(await _authenticationService.AutoSignIn(confirmEmail.Email));
                }
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = "Error occured" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetUser)
        {
            try
            {
                var result = await _authenticationService.ResetPassword(resetUser.Email, resetUser.Token, resetUser.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = "Error occured" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] AuthEmailDTO sendEmail)
        {
            try
            {
                await ProcessEmail(sendEmail.Email, sendEmail.Type);
                return Ok();
            }
            catch (Exception)
            {
                return Ok();
            }
        }

        private async Task ProcessEmail(string email, string type)
        {
            string token = await _authenticationService.GenerateEmailToken(email, type);
            string link = _configuration.GetValue<string>("PortalURL") + "Reset?email=" + email + "&token=" + token;
            string subject = "WaterAlerts Password Reset";
            string body = "A password reset has been requested on your Hydro Tasmania water alerts account. If this wasn't you please contact us at: contactus@hydro.com.au<br /><br />" +
                    "Please click the following link to reset your password.<br /><br /><a target='_blank' href='" + link + "'>Reset Password</a><br /><br />" +
                    "Or you can copy the following URL and paste it into your web browser.<br /><br />" + link +
                    "<br /><br />Please do not reply to this message.<br /><br />Thank you,<br /><br />Hydro Tasmania";

            if (type.ToLower() == "confirm")
            {
                link = _configuration.GetValue<string>("PortalURL") + "Confirm?email=" + email + "&token=" + token;
                subject = "WaterAlerts Account Activation";
                body = "Thanks for registering to Hydro Tasmania's water alerts service.<br /><br />Please click the following link to confirm your email.<br /><br />" +
                    "<a target='_blank' href='" + link + "'>Confirm Email</a><br /><br />Or you can copy the following URL and paste it into your web browser.<br /><br />" + link +
                    "<br /><br />If you believe you received this email in error, please contact contactus@hydro.com.au for assistance.<br /><br />Please do not reply to this message." +
                    "<br /><br />Thank you,<br /><br />Hydro Tasmania";
            }
            _smtpClient.Send(email, subject, body);
        }
    }
}
