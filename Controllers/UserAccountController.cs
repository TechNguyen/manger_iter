using System.Reflection.Emit;
using System.Reflection.Metadata;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using OtpNet;
using StackExchange.Redis;
using It_Supporter.DataRes;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;

        private readonly IUserAccount _userAccount;

        private readonly IEmaiLService _emailservice;

        private UserAccountContext _UserAccountContext;

        private readonly ITokenService _tokenService;

        private readonly IConfiguration _configuration;

        public int remainTiming {set;get;}

        public UserAccountController(ILogger<UserAccountController> logger,
            IEmaiLService emailservice,
            UserAccountContext _userAccountContext,
            IConfiguration configuration,
            ITokenService tokenservice,
            IUserAccount userAccount)
        {
            _logger = logger;
            _UserAccountContext = _userAccountContext;
            _configuration = configuration;
            _emailservice = emailservice;
            _userAccount = userAccount;
            _tokenService = tokenservice;

        }

        //[HttpGet("role")]
        //public async Task<IActionResult> getAllRoles() {
        //    try {
        //        var rs = _tokenService.GetAllRoles();
        //        return Ok(rs);
        //    } catch (Exception ex) {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccountCon([FromBody] UserAccount accregister, [FromQuery] string role)
        {
            try
            {
                var rs = await _userAccount.createAccount(accregister, role);
                ProducerResponse producer = new ProducerResponse { 
                    statuscode =rs.Key,
                    message = rs.Value
                
                };

                return Ok(producer);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //login with user
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAccount([FromBody] Login login)
        {
            try
            {
                var auth = await _userAccount.Login(login, _configuration);
                AuthResult producer = new AuthResult();
                producer.statuscode = auth.statuscode;
                producer.message = auth.message;
                producer.AccessToken = auth.AccessToken;
                producer.RefreshToken = auth.RefreshToken;
                return Ok(producer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // send Otp in Email
        [HttpPost("sendMail")]
        [ProducesResponseType(200, Type = typeof(ProducerResAddPost))]
        public async Task<IActionResult> SendMessageMail([FromBody] UserEmailOption userEmailOption)
        {
            try
            {
                await _emailservice.SendToEmail(userEmailOption);
                ProducerResAddPost result = new ProducerResAddPost
                {
                    statuscode = 200,
                    message = "Send Email Successfully!"
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error {ex.Message}");
            }
        }

        [HttpPost("sendmail-to-resetpass")]
        public async Task<IActionResult> SendMailToResetPass([FromBody] string email)
        {
            try
            {
                
                var res = await _emailservice.SendMailToReset(email); 

                ProducerResponse producer = new ProducerResponse();
                if(res)
                {
                    producer.statuscode = 200;
                    producer.message = "Send Email successfully!";
                } else
                {
                    producer.statuscode = 403;
                    producer.message = "Send Email unsucessfully!";
                }
                return Ok(producer);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost("changePass")]
        [ProducesResponseType(200, Type = typeof(ProducerResAddPost))]
        public async Task<IActionResult> changePass([FromBody] ResetPassword reset)
        {
            var isChange = await _emailservice.ChangePassWord(reset);
            ProducerResAddPost resultchange = new ProducerResAddPost();
            if (isChange.Succeeded)
            {
                resultchange.statuscode = 200;
                resultchange.message = "Change Password successfully";
                return Ok(resultchange);
            }
            else
            {
                resultchange.statuscode = 403;
                if (isChange != null)
                {
                    string error = "";
                    foreach (var err in isChange.Errors)
                    {
                        error += err.Description;
                    }
                    resultchange.message = error;
                } else
                {
                    resultchange.message = "Change password has error!";
                }
                return NotFound(resultchange);
            }
        }


        //[HttpPost("token/refresh")]
        //public async Task<IActionResult> RefreshToken([FromBody] RefreshToken refreshToken, IConfiguration configuration)
        //{
        //    try
        //    {
        //        var rs = await _tokenService.Refresh(refreshToken, configuration);
        //        AuthResult producer = new AuthResult();
        //        producer.message = rs.message;
        //        producer.statuscode = rs.statuscode;
        //        producer.RefreshToken = rs.RefreshToken;
        //        producer.AccessToken = rs.AccessToken;
        //        producer.errors = rs.errors;
        //        return Ok(producer);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
