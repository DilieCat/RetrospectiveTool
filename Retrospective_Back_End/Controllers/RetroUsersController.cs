using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Retrospective_Back_End.Models;
using Retrospective_Core.Models;

namespace Retrospective_Back_End.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class RetroUsersController : ControllerBase
    {
        private readonly UserManager<RetroUser> _userManager;

        public RetroUsersController(UserManager<RetroUser> userMgr)
        {
            _userManager = userMgr;
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

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Logining in an existing user
        /// </summary>
        /// <param name="model"></param>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] RegistrationModel model)
        {
            RetroUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

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

            return BadRequest();
        }


        private bool IsValid(string s)
        {
            return !string.IsNullOrEmpty(s);
        }
    }
}