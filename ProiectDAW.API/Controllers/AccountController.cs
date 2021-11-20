using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProiectDAW.API.Data;
using ProiectDAW.CommunicationObjects.Models.DTOs;
using ProiectDAW.CommunicationObjects.Models.DTOs.Authentication;
using ProiectDAW.CommunicationObjects.Models.UserModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProiectDAW.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IConfiguration _config;
        private readonly DatabaseContext _databaseContext;

        public AccountController(IConfiguration config, DatabaseContext databaseContext)
        {
            _config = config;
            _databaseContext = databaseContext;
        }

        #region Authentication
        [AllowAnonymous]
        [Route("[action]")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginDTO login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user == null)
                return NotFound("Username or password incorrect");
            else
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = $"Bearer {tokenString}" });
            }

            return response;
        }

        [AllowAnonymous]
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            if (register == null || string.IsNullOrEmpty(register.Username) ||
                string.IsNullOrEmpty(register.Password) || string.IsNullOrEmpty(register.EmailAddress))
            {
                return BadRequest("Fields cannot be null");
            }

            await _databaseContext.Users.AddAsync(new User
            {
                Username = register.Username,
                Password = register.Password,
                EmailAddress = register.EmailAddress,
                DateOfJoing = DateTime.Now,
                RoleId = 2
            });
            await _databaseContext.SaveChangesAsync();

            return Ok("User created successfully");
        }
        #endregion

        #region UserDetails
        [HttpGet]
        [Route("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUserDetails(string username)
        {
            var user = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Username == username);
            _databaseContext.Entry(user).Reference(x => x.Role).Load();
            _databaseContext.Entry(user).Reference(x => x.UserDetails).Load();

            var role = new RoleDTO
            {
                RoleId = user.Role.RoleId,
                Name = user.Role.Name
            };

            UserDetailsDTO userDetails = null;
            if (user.UserDetails != null)
            {
                userDetails.Firstname = user.UserDetails.Firstname;
                userDetails.Lastname = user.UserDetails.Lastname;
                userDetails.Address = user.UserDetails.Address;
                userDetails.PostalCode = user.UserDetails.PostalCode;
            }

            return Ok(new UserDTO
            {
                Id = user.Id,
                Username = username,
                EmailAddress = user.EmailAddress,
                DateOfJoing = user.DateOfJoing,
                Role = role,
                UserDetails = userDetails
            });
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<User>> GetCurrentUserDetails()
        {
            var username = GetUsername();
            var user = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Username == username);
            _databaseContext.Entry(user).Reference(x => x.Role).Load();
            _databaseContext.Entry(user).Reference(x => x.UserDetails).Load();

            var role = new RoleDTO
            {
                RoleId = user.Role.RoleId,
                Name = user.Role.Name
            };

            UserDetailsDTO userDetails = null;
            if (user.UserDetails != null)
            {
                userDetails.Firstname = user.UserDetails.Firstname;
                userDetails.Lastname = user.UserDetails.Lastname;
                userDetails.Address = user.UserDetails.Address;
                userDetails.PostalCode = user.UserDetails.PostalCode;
            }

            return Ok(new UserDTO
            {
                Id = user.Id,
                Username = username,
                EmailAddress = user.EmailAddress,
                DateOfJoing = user.DateOfJoing,
                Role = role,
                UserDetails = userDetails
            });
        } 
        #endregion

        [HttpPut]
        [Route("[action]")]
        [Authorize]
        public async Task<IActionResult> ChangeEmail(int id, string email)
        {
            var currentUser = await _databaseContext.Users.SingleAsync(x => x.Username == GetUsername());
            if (GetRole() != "Admin" && currentUser.Id != id)
                return Unauthorized("Cannot modify other peoples email or you are not an admin");

            var user = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound("User doesn't exist");

            user.EmailAddress = email;
            await _databaseContext.SaveChangesAsync();

            return Ok("Email changed succesfully");
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize]
        public async Task<IActionResult> ChangeDetails(int id, [FromBody] UserDetailsDTO details)
        {
            var currentUser = await _databaseContext.Users.SingleAsync(x => x.Username == GetUsername());
            if (GetRole() != "Admin" && currentUser.Id != id)
                return Unauthorized("Cannot modify other peoples email or you are not an admin");

            if (details == null)
                return BadRequest("Body cannot be null");

            var user = await _databaseContext.Users.Include(x => x.UserDetails).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return BadRequest("User doesn't exist");
            if (user.UserDetails == null) 
            {
                user.UserDetails = new UserDetails
                {
                    Firstname = details.Firstname ?? null,
                    Lastname = details.Lastname ?? null,
                    Address = details.Address ?? null,
                    PostalCode = details.PostalCode ?? null
                };

                await _databaseContext.SaveChangesAsync();

                return Ok("User details added succesfully");
            }

            if (details.Firstname != null)
                user.UserDetails.Firstname = details.Firstname;
            if (details.Lastname != null)
                user.UserDetails.Lastname = details.Lastname;
            if (details.Address != null)
                user.UserDetails.Address = details.Address;
            if (details.PostalCode != null)
                user.UserDetails.PostalCode = details.PostalCode;
            
            await _databaseContext.SaveChangesAsync();

            return Ok("User details changed succesfully");
        }

        #region Helper Methods
        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Data that the token contains
            var claims = new[]
            {
                new Claim(type: "Username", userInfo.Username),
                new Claim(ClaimTypes.Role, _databaseContext.Roles.FirstOrDefault(x=>x.RoleId==userInfo.RoleId).Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                                             _config["Jwt:Issuer"],
                                             claims,
                                             expires: DateTime.Now.AddMinutes(120),
                                             signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User AuthenticateUser(LoginDTO login)
        {
            User user = null;

            //TODO: Implement DB Logic
            //Validate the User Credentials

            user = _databaseContext.Users.FirstOrDefault(x => x.Username == login.Username && x.Password == login.Password);

            return user ?? null;
        }

        private string GetUsername()
        {
            return HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Username").Value;
        }

        private string GetRole()
        {
            return HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
        }
        #endregion
    }
}