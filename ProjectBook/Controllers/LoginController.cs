using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectBook.Data;
using ProjectBook.Request;
using ProjectBook.response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace ProjectBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(ApiDbContext dbContext, IConfiguration config) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        private readonly IConfiguration _config = config;

        [HttpPost]
        public IActionResult Login([FromBody] AuthRequest user)
        {
            var currentUser = _dbContext.Users.SingleOrDefault(u => u.Email == user.Email);
            if (currentUser == null || !BC.Verify(user.Password, currentUser.Password))
            {
                return NotFound();
            }

            // JWT
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, currentUser.Email),
                new Claim(ClaimTypes.Role, currentUser.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            //Response
            var authResponse = new AuthResponse
            {
                Access_token = jwt,
                Info = new UserResponse()
            };
            authResponse.Info.Id = currentUser.Id;
            authResponse.Info.Name = currentUser.Name;
            authResponse.Info.Email = currentUser.Email;
            authResponse.Info.Photo = currentUser.Photo;
            authResponse.Info.Role = currentUser.Role;
            return Ok(authResponse);

        }
    }
}
