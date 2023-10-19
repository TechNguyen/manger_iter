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

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;

        private readonly IUserAccount _userAccount;

        private readonly IEmaiLService _emailservice;

        private UserAccountContext _UserAccountContext;


        private readonly IConfiguration _configuration;

        public int remainTiming {set;get;}

        public UserAccountController(ILogger<UserAccountController> logger,IEmaiLService emailservice , IUserAccount userAccount, UserAccountContext _userAccountContext, IConfiguration configuration)
        {
            _userAccount = userAccount;
            _logger = logger;
            _UserAccountContext = _userAccountContext;
            _configuration = configuration;
            _emailservice = emailservice;
        }

        //login with user
        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        public IActionResult LoginAccount([FromBody] Login login)
        {
            var token = _userAccount.Login(login, _configuration);
            if(token == "")
            {
                return NotFound(ModelState);
            }
            return Ok(token);
        }
        // send Otp in Email
        [HttpPost("sendMail")]
        [ProducesResponseType(200, Type = typeof(ProducerResAddPost))]
        public async Task<IActionResult> SendMessageMail([FromBody] UserEmailOption userEmailOption) {
            try {
                await _emailservice.SendToTest(userEmailOption, _configuration);
                ProducerResAddPost result = new ProducerResAddPost {
                    statuscode = 200,
                    message = "Send Email Successfully!"
                };
                return Ok(result);
            } catch (Exception ex) {
                return StatusCode(500, $"Error{ex.Message}");
            }
        }
        //
        [HttpPost("otp")]
        public IActionResult GenterraOtpCode([FromQuery] string secretkey) {
            var otp = _emailservice.GenerateOtp(secretkey);
            return Ok(otp);
        }

        [HttpPost("checkOtp")]
        [ProducesResponseType(200, Type = typeof(ProducerResAddPost))]
        public  IActionResult CheckOtpGen([FromBody] checkOtp check ) {
            var otpcheck = _emailservice.CheckOtp(check, _configuration);
            ProducerResAddPost resultcheck = new ProducerResAddPost();
            if(otpcheck.statusCode == 200) {
                resultcheck.statuscode = 200;
                resultcheck.message = "Otp right";
                return Ok(resultcheck);
            }
            else {
                if(otpcheck.statusCode == 400) {
                    resultcheck.statuscode = 400;
                    resultcheck.message = "Otp fault";
                } else {
                    resultcheck.statuscode = 405;
                    resultcheck.message = "Otp het han";
                }
                return NotFound(resultcheck);
            };
        }

        [HttpPost("changePass")]
        [ProducesResponseType(200, Type = typeof(ProducerResAddPost))]
        [ProducesResponseType(400)]
        public IActionResult changePass ([FromBody] OtpSend otpSend) {
            var isChange = _emailservice.ResetPassword(otpSend ,_configuration);
            ProducerResAddPost resultchange = new ProducerResAddPost();
            if(isChange) {
                resultchange.statuscode = 200;
                resultchange.message = "Change Password successfully";
                return Ok(resultchange);
            } else {
                resultchange.statuscode = 400;
                resultchange.message = "Change Password fault!";
                return NotFound(resultchange);
            }
        } 
    }
}
