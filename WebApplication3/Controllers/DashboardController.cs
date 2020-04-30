using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var user = await _context.Users.FindAsync(id);        
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post(User _userData)
        {
            var user = await GetUser(_userData.Username, _userData.Password);
            //await Get(user.Id);
            return Ok(Get(user.Id));
        }


        private async Task<User> GetUser(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}