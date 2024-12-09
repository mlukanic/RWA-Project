using Marketplace.Dtos;
using Marketplace.Interfaces;
using Marketplace.Security;
using MarketplaceClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MarketplaceContext _context;
        private readonly ILogService _logService;

        public UserController(IConfiguration configuration, MarketplaceContext context, ILogService logService)
        {
            _configuration = configuration;
            _context = context;
            _logService = logService;
        }

        [HttpGet("[action]")]
        public ActionResult GetToken()
        {
            try
            {
                // The same secure key must be used here to create JWT,
                // as the one that is used by middleware to verify JWT
                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 10);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserDto> Register(UserDto userDto)
        {
            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = userDto.Username.Trim();
                if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                var userRole = _context.Roles.FirstOrDefault(x => x.RoleName == "User");

                // Hash the password
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, b64salt);

                // Create user from DTO and hashed password
                var user = new User
                {
                    UserId = userDto.Id,
                    Username = userDto.Username,
                    PwdHash = b64hash,
                    PwdSalt = b64salt,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Phone = userDto.Phone,
                    RoleId = userRole.RoleId
                };

                // Add user and save changes to database
                _context.Add(user);
                _context.SaveChanges();

                // Update DTO Id to return it to the client
                userDto.Id = user.UserId;
                _logService.Log("INFO", $"User with username: {user.Username} successfully registered");
                return Ok(userDto);

            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error while registering a user: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult Login(UserLoginDto userDto)
        {
            try
            {
                var genericLoginFail = "Incorrect username or password";

                // Try to get a user from database
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == userDto.Username);
                if (existingUser == null)
                    return BadRequest(genericLoginFail);

                // Check is password hash matches
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, existingUser.PwdSalt);
                if (b64hash != existingUser.PwdHash)
                    return BadRequest(genericLoginFail);

                // Create and return JWT token
                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 120, userDto.Username);

                _logService.Log("INFO", $"User with username: {userDto.Username} successfully logged in");
                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error while logging in: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserChangePasswordDto> ChangePassword(UserChangePasswordDto userChangePasswordDto)
        {
            try
            {
                var trimmedUsername = userChangePasswordDto.Username.Trim();
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == trimmedUsername);
                if (existingUser == null)
                {
                    return BadRequest($"User {trimmedUsername} not found");
                }

                existingUser.PwdSalt = PasswordHashProvider.GetSalt();
                existingUser.PwdHash = PasswordHashProvider.GetHash(userChangePasswordDto.Password, existingUser.PwdSalt);

                _context.Update(existingUser);
                _context.SaveChanges();

                _logService.Log("INFO", $"Password for user with username: {userChangePasswordDto.Username} has been successfully changed");
                return Ok();
            }

            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error while changing password: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserDto> PromoteUser(UserPromoteDto userPromoteDto)
        {
            try
            {
                var trimmedUsername = userPromoteDto.Username.Trim();
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == trimmedUsername);
                if (existingUser == null)
                {
                    return BadRequest($"User with username: {trimmedUsername} not found");
                }

                var adminRole = _context.Roles.FirstOrDefault(x => x.RoleName == "Admin");

                existingUser.RoleId = adminRole.RoleId;
                _context.Update(existingUser);
                _context.SaveChanges();

                _logService.Log("INFO", $"User with username: {userPromoteDto.Username} has been successfully promoted");
                return Ok();
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error while promoting a user: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
