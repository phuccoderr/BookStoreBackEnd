using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;
using ProjectBook.Data;
using ProjectBook.Models;
using ProjectBook.response;
using ProjectBook.Request;
using Microsoft.AspNetCore.Authorization;
using ProjectBook.Helpers;

namespace ProjectBook.Controllers
{
    [Route("api/auth/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class UsersController(ApiDbContext dbContext, IConfiguration config,CloudinaryService cloudinaryService) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        private readonly IConfiguration _config = config;
        private readonly CloudinaryService _cloudinaryService = cloudinaryService;

        [HttpPost]
        public IActionResult Register([FromForm] UserDTORequest userRequest)
        {
            var userExists = _dbContext.Users.FirstOrDefault(u => u.Email == userRequest.Email);
            if (userExists != null)
            {
                return BadRequest("Email này đã tồn tại, Vui lòng nhập email khác!");
            }
            string passwordHash = BC.HashPassword(userRequest.Password);
            // CREATE USER
            var user = new User();
            user.Password = passwordHash;

            // CLOUDINARY
            if (userRequest.File != null)
            {
                var uploadResult = _cloudinaryService.SaveImage(userRequest.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Lỗi khi upload hình ảnh");

                }
                user.Photo = uploadResult.SecureUrl.ToString();
            }
            
            user.Email = userRequest.Email;
            user.Name = userRequest.Name;   
            user.Enabled = userRequest.Enabled;
            user.Role = userRequest.Role;

           

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);

        }

        [HttpGet("{id}")]
        
        public IActionResult getUser(int id) {
            var currentUser = _dbContext.Users.Find(id);
            if (currentUser == null) {
                return NotFound();
            }
            return Ok(EntityToDTO(currentUser));
        }

        [HttpGet("page/{pageNumber}")]
        public IActionResult getUsers(string sort, int pageNumber = 1, string keyword = null) {
            IQueryable<User> users;
            switch(sort)
            {
                case "desc":
                    users = _dbContext.Users.OrderByDescending(u => u.Id);
                    break;
                case "asc":
                    users = _dbContext.Users.OrderBy(u => u.Id);
                    break;
                default:
                    users = _dbContext.Users;
                    break;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                users = users.Where(u => u.Name.Contains(keyword) || u.Email.Contains(keyword));
            }
 
            var currentPageNumber = pageNumber;
            var currentPageSize = 2;
            int totalItems = users.Count();

            int startCount = (pageNumber - 1) * currentPageSize + 1;
            int endCount = startCount + currentPageSize - 1;

            int totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);
            
            // Kiểm tra giá trị trang hợp lệ
            currentPageNumber = Math.Max(1, Math.Min(currentPageNumber, totalPages));

            var pagedUsers = users.Skip((currentPageNumber - 1) * currentPageSize)
                          .Take(currentPageSize)
                          .ToList();

           

            var pageResponse = new UserPageResponse()
            {
                Users = listEntityToDTOs(pagedUsers.ToList())
            };
            pageResponse.Total_pages = totalPages;
            pageResponse.Total_items = totalItems;
            pageResponse.Current_page = currentPageNumber;
            pageResponse.Start_count = startCount;
            pageResponse.End_count = endCount;   
            return Ok(pageResponse);
        }

        [HttpPut("{id}")]
        public IActionResult PutUsers(int id,[FromForm] UserDTORequest userRequest)
        {
            var currentUser = _dbContext.Users.Find(id);
            if (currentUser == null)
            {
                return NotFound();
            }
/*            if (!BC.Verify(userRequest.Password, currentUser.Password))
            {
                currentUser.Password = BC.HashPassword(userRequest.Password);
            }*/

            // CLOUDINARY
            if (userRequest.File != null)
            {
                _cloudinaryService.deleteImage(currentUser.Photo);
                var uploadResult = _cloudinaryService.SaveImage(userRequest.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Lỗi khi upload hình ảnh");

                }
                currentUser.Photo = uploadResult.SecureUrl.ToString();
            }
            currentUser.Email = userRequest.Email;
            currentUser.Name = userRequest.Name;
            currentUser.Enabled = userRequest.Enabled;
            currentUser.Role = userRequest.Role;
            _dbContext.SaveChanges();

            return Ok(EntityToDTO(currentUser));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUsers(int id)
        {
            var currentUser = _dbContext.Users.Find(id);
            _cloudinaryService.deleteImage(currentUser.Photo);
            if (currentUser == null)
            {
                return NotFound();
            }
            _dbContext.Users.Remove(currentUser);
            _dbContext.SaveChanges();
            return Ok();
        }

        

        private List<UserResponse> listEntityToDTOs(List<User> users)
        {
            List<UserResponse> dtos = new List<UserResponse>();
            foreach (var item in users)
            {
                dtos.Add(EntityToDTO(item));
            }
            return dtos;
        }

        private UserResponse EntityToDTO(User user)
        {
            UserResponse userResponse = new UserResponse()
            {
                Email = user.Email,
                Name = user.Name,
                Id = user.Id,
                Photo = user.Photo,
                Enabled = user.Enabled,
                Role = user.Role,
            };
            return userResponse;
        }

    }
}
