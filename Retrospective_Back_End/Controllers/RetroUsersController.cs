using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Retrospective_Back_End.Models;
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

        public RetroUsersController(UserManager<RetroUser> userMgr,
            SignInManager<RetroUser> signInMgr,
            IRetroRespectiveRepository repo)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            _repo = repo;
        }

        [HttpPost("register")]
        public async  Task<ActionResult> Register([FromBody] RegistrationModel model)
        {
           if(ModelState.IsValid && IsValid(model.Email) && IsValid(model.Password))
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
                } else
                {
                    return BadRequest();
                }
            } else
            {
                return BadRequest();
            }
        } 

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

            return BadRequest();
        }


        private bool IsValid(string s)
        {
            return s != null && s.Length != 0;
        }
    }
}