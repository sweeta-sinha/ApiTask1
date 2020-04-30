using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using WebApplication3.Models;

namespace SimpleApiTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public TokenController(IConfiguration config, ApplicationDbContext context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(User _userData)
        {

            if (_userData != null && _userData.Username != "" && _userData.Password != "")
            {
                var user = await GetUser(_userData.Username, _userData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                     new Claim("Id", user.Id.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("UserName", user.Username),
                    new Claim("Password", user.Password)
                   };

                    var claim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                     new Claim("Id", user.Id.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("UserName", user.Username),
                    new Claim("Password", user.Password)
                    };
                    
                    var claimsIdentity = new ClaimsIdentity(
                        claim, CookieAuthenticationDefaults.AuthenticationScheme);
                    ////var claimsIdentity = new ClaimsIdentity(new[]
                    ////{
                    ////     new Claim(ClaimTypes.Name, user.Username),

                    ////     //...
                    //// }, "auth_cookie");

                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);


                    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    // await Request.HttpContext.SignInAsync("Cookies", claimsPrincipal);
                    //await HttpContext.SignInAsync(
                    //                     CookieAuthenticationDefaults.AuthenticationScheme,
                    //                     new ClaimsPrincipal(claimsIdentity));

                    Set("MyCookie",(token.EncodedHeader +"." +token.EncodedPayload+"."+token.EncryptingCredentials), 10);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    
                     return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest("Fill the fields");
            }
        }
        public void Set(string key, string value, int? expireTime)
        {

            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            Response.Cookies.Append(key, value, option);
        }

        private async Task<User> GetUser(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}