using System;
using System.Collections.Generic;
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
        public ActionResult<IEnumerable<string>> Get()
        {

            // Request.Cookies["x"];
            if (Request.Cookies["x"] != null)
            {
                var name = HttpContext.Request.Cookies["x"];
                ///var decoded = jwt_decode(name);
                string s = "{\"data\":[[\"Tiger Nixon\",\"System Architect\", \"Edinburgh\", \"5421\",\"2011/04/25\",\"$320,800\"],[\"Garrett Winters\",\"Accountant\",\"Tokyo\", \"8422\",\"2011/07/25\",\"$170,750\"]]}";

                return Ok(JObject.Parse(s));
            }

            else
            {
                return BadRequest("Login First");
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> Post(User _userData)
        //{
        //    var user = await GetUser(_userData.Username, _userData.Password);
        //    //await Get(user.Id);
        //    return Ok(Get(user.Id));
        //}
     
        private async Task<User> GetUser(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}