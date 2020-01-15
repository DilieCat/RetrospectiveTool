using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Retrospective_Back_End.Models;
using Retrospective_Back_End.Services;
using Retrospective_Back_End.Utils;
using Retrospective_Core.Models;
using Retrospective_Core.Services;

namespace Retrospective_Back_End.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class RetroUsersController : ControllerBase
    {
        private readonly UserManager<RetroUser> userManager;
        private readonly SignInManager<RetroUser> signInManager;
        private IRetroRespectiveRepository _repo;
        private readonly IDecoder decoder;

        public RetroUsersController(UserManager<RetroUser> userMgr,
            SignInManager<RetroUser> signInMgr,
            IRetroRespectiveRepository repo,
            IDecoder decoder)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            _repo = repo;
            this.decoder = decoder;
        }


        /// <summary>
        /// Registering a new user
        /// </summary>
        /// <param name="model"></param>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegistrationModel model)
        {
            if (ModelState.IsValid && IsValid(model.Email) && IsValid(model.Password))
            {
                RetroUser user = new RetroUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    LockoutEnabled = false
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(409);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Logining in an existing user
        /// </summary>
        /// <param name="model"></param>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] RegistrationModel model)
        {
            RetroUser user = await userManager.FindByEmailAsync(model.Email);

            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecureKey"));

                var token = new JwtSecurityToken(
                    claims: authClaims,
                    expires: DateTime.Now.AddHours(24)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            } 

            return Unauthorized();
        }

        /// <summary>
        /// To recover account for sending a mail to the user with a password reset link in frontend
        /// </summary>
        /// <param name="recoveryViewModel"></param>
        [HttpPost("recovery")]
        public async Task<ActionResult> AccountRecovery([FromBody] RecoveryViewModel recoveryViewModel)
        {
            if (IsValid(recoveryViewModel.Email))
            {
                var user = await userManager.FindByEmailAsync(recoveryViewModel.Email);

                if (user != null)
                {
                    var authClaims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecureKey"));

                    var token = new JwtSecurityToken(
                        claims: authClaims,
                        expires: DateTime.Now.AddMinutes(30)
                    );

                    SendGridEmailService.ExecuteSendRecoveryEmail(user.Email, new JwtSecurityTokenHandler().WriteToken(token)).Wait();

                    return Ok(new
                    {
                        message = MessageConstants.AccountRecoveryOk
                    });
                }
            }

            return BadRequest(new
            {
                message = MessageConstants.AccountRecoveryBad
            });
        }

        /// <summary>
        /// Updates the password of a RetroUser
        /// </summary>
        /// <param name="token"></param>
        /// <param name="passwordViewModel"></param>
        [HttpPost("updatepassword/{token}")]
        public async Task<ActionResult> UpdatePassword(string token, [FromBody] UpdatePasswordViewModel passwordViewModel)
        {
            // First checktoken
            // TODO: Add token check
            var retroUserId = decoder.DecodeToken(token);

            var retroUser = await userManager.FindByIdAsync(retroUserId);

            if (retroUser != null)
            {
                var result = await userManager.RemovePasswordAsync(retroUser);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(retroUser, passwordViewModel.Password);
                    if (result.Succeeded)
                    {
                        return Ok(new
                        {
                            message = MessageConstants.ResetPasswordOk
                        });
                    }
                }
                else
                {
                    return BadRequest(new
                    {
                        message = MessageConstants.ResetPasswordError
                    });
                }
            }

            return BadRequest(new
            {
                message = MessageConstants.ResetPasswordBad
            });
        }

        private bool IsValid(string s)
        {
            return !string.IsNullOrEmpty(s);
        }
    }
}