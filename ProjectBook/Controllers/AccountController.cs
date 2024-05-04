using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBook.Data;
using ProjectBook.Helpers;
using ProjectBook.Models;
using ProjectBook.Request;
using ProjectBook.response;
using BC = BCrypt.Net.BCrypt;
namespace ProjectBook.Controllers
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class AccountController(ApiDbContext dbContext, CloudinaryService cloudinaryService) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        private readonly CloudinaryService _cloudinaryService = cloudinaryService;

        [HttpPost]
        [Route("{id}")]
        [Authorize]
        public IActionResult UpdateAccount([FromForm] UserDTORequest userRequest,int id)
        {
            var currentUser = _dbContext.Users.Find(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            if (userRequest.Password != null)
            {
                if (!BC.Verify(userRequest.Password, currentUser.Password))
                {
                    currentUser.Password = BC.HashPassword(userRequest.Password);
                }
            }

            // CLOUDINARY
            if (userRequest.File != null)
            {
                var uploadResult = _cloudinaryService.SaveImage(userRequest.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Lỗi khi upload hình ảnh");

                }
                currentUser.Photo = uploadResult.SecureUrl.ToString();
            }
            currentUser.Name = userRequest.Name;
            currentUser.Enabled = userRequest.Enabled;

            _dbContext.SaveChanges();
            return Ok(EntityToDTO(currentUser));

        }

        private UserResponse EntityToDTO(User user)
        {
            UserResponse userResponse = new UserResponse()
            {
                Email = user.Email,
                Name = user.Name,
                Id = user.Id,
                Photo = user.Photo,
                Enabled = user.Enabled
            };
            return userResponse;
        }
    }
}
