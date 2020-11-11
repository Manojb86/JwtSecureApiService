using JwtSecureApiService.DatabaseContext;
using JwtSecureApiService.Helpers;
using JwtSecureApiService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace JwtSecureApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserContext userContext;
        private readonly ITokenService tokenService;

        public AuthController(UserContext userContext, ITokenService tokenService)
        {
            this.userContext = userContext;
            this.tokenService = tokenService;
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel != null && !string.IsNullOrEmpty(loginModel.UserName) && !string.IsNullOrEmpty(loginModel.Password))
            {
                var user = userContext.LoginModels.FirstOrDefault(x => x.UserName == loginModel.UserName && x.Password == loginModel.Password);
                if (user != null)
                {                   
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, "Manager")
                    };

                    string jwtToken = this.tokenService.GenerateAccessToken(claims);
                    string refreshToken = this.tokenService.GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                    userContext.SaveChanges();

                    return Ok(new { AccessToken = jwtToken, RefreshToken = refreshToken});
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return BadRequest("Login details not found");
            }
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult GetRefreshToken(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }

            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            userContext.SaveChanges();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpPost]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var user = userContext.LoginModels.SingleOrDefault(u => u.UserName == username);

            if (user == null) return BadRequest();

            user.RefreshToken = null;
            userContext.SaveChanges();

            return NoContent();
        }
    }
}