using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using WebApplication3.Models;

namespace SimpleApiTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public DashboardController(IConfiguration config, ApplicationDbContext context)
        {
            _configuration = config;
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAsync()
        {

         
                var name = HttpContext.Request.Cookies["x"];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(name);
                var user = token.Claims.First(claim => claim.Type == "UserName").Value;
                var pwd = token.Claims.First(claim => claim.Type == "Password").Value;
                await GetUser(user, pwd);
                if((user !="") && (pwd != ""))
                {
                    string s = "{\"data\":[[\"Tiger Nixon\",\"System Architect\", \"Edinburgh\", \"54\",\"2011/04/25\",\"$320,800\"],[\"Garrett Winters\",\"Accountant\",\"Tokyo\", \"22\",\"2011/07/25\",\"$170,750\"]]}";

                    return Ok(JObject.Parse(s));
                }
           
            else
            {
               
                return BadRequest("Login First");
            }
        }
           
        private async Task<User> GetUser(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}