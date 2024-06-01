using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProjectBook.Data;
using ProjectBook.Helpers;
using ProjectBook.Models;
using ProjectBook.Request;
using ProjectBook.response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using CloudinaryDotNet;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ProjectBook.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectBook.Controllers.Frontend
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class CustomersController(ApiDbContext dbContext, CloudinaryService cloudinaryService, IConfiguration config) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;
        private readonly CloudinaryService _cloudinaryService = cloudinaryService;
        private readonly IConfiguration _config = config;

        [HttpPost("/login")]
        public IActionResult CustomerLogin(CustomerDTORequest customerRequest)
        {
            var currentCustomer = _dbContext.Customers.FirstOrDefault(c => c.Email == customerRequest.Email);
            if (currentCustomer == null || !BC.Verify(customerRequest.Password, currentCustomer.Password))
            {
                return NotFound();
            }
            if (currentCustomer.Password == null)
            {
                return NotFound();
            }
            
            if (currentCustomer.Enabled == false)
            {
                return NotFound();
            }
            
            // JWT
            var jwt = GenerateToken(currentCustomer.Email);

            //Response
            var authResponse = new AuthResponse
            {
                Access_token = jwt,
                Info = new UserResponse()
            };
            authResponse.Info.Id = currentCustomer.Id;
            authResponse.Info.Name = currentCustomer.Name;
            authResponse.Info.Email = currentCustomer.Email;
            authResponse.Info.Photo = currentCustomer.Photo;
            authResponse.Info.Enabled = currentCustomer.Enabled;
            return Ok(authResponse);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult updateCustomer([FromForm] string data, IFormFile image, int id)
        {
            var currentCustomer = _dbContext.Customers.Find(id);
            if (currentCustomer == null)
            {
                return NotFound();
            }

            //convert json -> object
            var customerRequest = JsonConvert.DeserializeObject<Customer>(data);
            var customerResponse = new UserResponse();
            customerResponse.Role = "oauth2";
            if (currentCustomer.Password != null)
            {
                if (customerRequest.Password != null && customerRequest.Password != "")
                {
                    currentCustomer.Password = BC.HashPassword(customerRequest.Password);
                }
                customerResponse.Role = null;
            }

            currentCustomer.Name = customerRequest.Name;
            
            if (image != null)
            {
                _cloudinaryService.deleteImage(currentCustomer.Photo);
                var uploadResult = _cloudinaryService.SaveImage(image);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Lỗi khi upload hình ảnh");

                }
                currentCustomer.Photo = uploadResult.SecureUrl.ToString();
            }

            
            customerResponse.Email = currentCustomer.Email;
            customerResponse.Name = currentCustomer.Name;
            customerResponse.Photo = currentCustomer.Photo;
            customerResponse.Id = currentCustomer.Id;
            customerResponse.Enabled = currentCustomer.Enabled;
            _dbContext.SaveChanges();
            return Ok(customerResponse);
        }

        [HttpPost("/register")]
        public IActionResult PostCustomer([FromBody] CustomerDTORequest customerRequest)
        {

            var currentCustomer = _dbContext.Customers.FirstOrDefault(c => c.Email == customerRequest.Email);
            if (currentCustomer != null)
            {
                return BadRequest(customerRequest.Email);
            }
            var customer = new Customer();
            customer.Email = customerRequest.Email;
            customer.Name = customerRequest.Name;
            customer.Enabled = true;
            customer.Created_Time = DateTime.Now;

            var passwordHash = BC.HashPassword(customerRequest.Password);
            customer.Password = passwordHash;

            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("/login-facebook")]
        public async Task<IActionResult> FbCallBack(string accessToken)
        {
            string userInfoUrl = $"https://graph.facebook.com/me?access_token={accessToken}&fields=name,email,picture";
            string json = "";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(userInfoUrl);
                var requestUri = response.RequestMessage.RequestUri;

                json = await response.Content.ReadAsStringAsync();
            }
            var accountFacebook = JsonConvert.DeserializeObject<AccountFacebook>(json);
            var jwt = GenerateToken(accountFacebook.Email);
            var authResponse = new AuthResponse
            {
                Access_token = jwt,
                Info = new UserResponse()
            };

            var currentAccountFB = _dbContext.Customers.FirstOrDefault(c => c.Email == accountFacebook.Email);
            if (currentAccountFB != null)
            {
                authResponse.Info.Id = currentAccountFB.Id;
                authResponse.Info.Name = currentAccountFB.Name;
                authResponse.Info.Email = currentAccountFB.Email;
                authResponse.Info.Photo = currentAccountFB.Photo;
                authResponse.Info.Enabled = currentAccountFB.Enabled;
                authResponse.Info.Role = "oauth2";
                return Ok(authResponse);
            }

            var customer = new Customer();
            customer.Email = accountFacebook.Email;
            customer.Name = accountFacebook.Name;
            customer.Photo = accountFacebook.Picture.Data.Url;
            customer.Enabled = true;
            customer.Created_Time = DateTime.Now;

            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();
            //Response
            authResponse.Info.Id = customer.Id;
            authResponse.Info.Name = customer.Name;
            authResponse.Info.Email = customer.Email;
            authResponse.Info.Photo = customer.Photo;
            authResponse.Info.Enabled = customer.Enabled;
            authResponse.Info.Role = "oauth2";
            return Ok(authResponse);
        }


       

        [HttpPost("/login-google")]
        public async Task<IActionResult> GoogleCallBack(string accessToken)
        {
            string userInfoUrl = "https://www.googleapis.com/oauth2/v3/userinfo";
            string json = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await client.GetAsync(userInfoUrl);
                json = await response.Content.ReadAsStringAsync();
                //convert json -> object
            }
            var accountGoogle = JsonConvert.DeserializeObject<AccountGoogle>(json);


            var jwt = GenerateToken(accountGoogle.Email);
            var authResponse = new AuthResponse
            {
                Access_token = jwt,
                Info = new UserResponse()
            };

            var currentAccountGoogle = _dbContext.Customers.FirstOrDefault(c => c.Email == accountGoogle.Email);
            if (currentAccountGoogle != null)
            {
                authResponse.Info.Id = currentAccountGoogle.Id;
                authResponse.Info.Name = currentAccountGoogle.Name;
                authResponse.Info.Email = currentAccountGoogle.Email;
                authResponse.Info.Photo = currentAccountGoogle.Photo;
                authResponse.Info.Enabled = currentAccountGoogle.Enabled;
                authResponse.Info.Role = "oauth2";
                return Ok(authResponse);
            }
            var customer = new Customer();
            customer.Email = accountGoogle.Email;
            customer.Name = accountGoogle.Name;
            customer.Photo = accountGoogle.Picture;
            customer.Enabled = true;
            customer.Created_Time = DateTime.Now;
            

            

            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();
            //Response
            authResponse.Info.Id = customer.Id;
            authResponse.Info.Name = customer.Name;
            authResponse.Info.Email = customer.Email;
            authResponse.Info.Photo = customer.Photo;
            authResponse.Info.Enabled = customer.Enabled;
            authResponse.Info.Role = "oauth2";
            return Ok(authResponse);
        }

       

        private string GenerateToken(string email)
        {
            // JWT
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        /* private async Task<string> AccessToken(string code)
        {
            string tokenURL = "https://accounts.google.com/o/oauth2/token";
            var dicData = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _config["Google:ClientId"] },
                { "client_secret", _config["Google:ClientSecret"] },
                { "redirect_uri", _config["Google:RedirectGoogle"] },
                { "grant_type", "authorization_code" }
            };

            try
            {
                using (var client = new HttpClient())
                using (var content = new FormUrlEncodedContent(dicData))
                {
                    HttpResponseMessage response = await client.PostAsync(tokenURL, content);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);

                    return json.GetValue("access_token").ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }*/

        /*[HttpGet("/login-google")]
       public IActionResult SignInGoogle()
       {
           UriBuilder uriBuilder = new UriBuilder("https://accounts.google.com/o/oauth2/v2/auth");
           var query = HttpUtility.ParseQueryString(uriBuilder.Query);
           query["client_id"] = _config["Google:ClientId"];
           query["response_type"] = "code";
           query["redirect_uri"] = _config["Google:RedirectGoogle"];
           query["scope"] = "openid email profile";
           uriBuilder.Query = query.ToString();

           Uri loginURI = uriBuilder.Uri;
           return Ok(loginURI.ToString());
       }*/

    }
}
