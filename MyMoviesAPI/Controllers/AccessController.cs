using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMoviesAPI.Data;
using MyMoviesAPI.Data.Entities;
using MyMoviesAPI.Models.User;
using MyMoviesAPI.Utility;
using System.Text;

namespace MyMoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IDatabaseAgent _agent;
        private readonly ILogger<MovieController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccessController(IDatabaseAgent agent, ILogger<MovieController> logger, IMapper mapper, IConfiguration configuration)
        {
            _agent = agent;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        // POST: api/Access/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult<UserDTO> PostLogin(string username, string password)
        {
            try
            {
                var user = _agent.GetUserByUsername(username);
                bool authenticated = Codec.VerifyHash(Encoding.UTF8.GetString(Convert.FromBase64String(password)), user != null ? user.Password : ""); 
                if(user != null && authenticated)
                {
                    return Ok(_mapper.Map<UserDTO>(user));
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve user. Error: {ex}");
                return Unauthorized();
            }
        }

        // POST: api/Access/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<UserDTO> PostRegister(UserCreateDTO user)
        {
            try
            {
                if (_agent.UserExists(user.Username, user.Email))
                {
                    return BadRequest($"Failed to create a user. Username or email might already exists.");
                }
                User newUser = _mapper.Map<User>(user);
                newUser.Status = UserStatus.ACTIVE;
                newUser.AccountCreated = DateTime.Now;
                newUser.Type = UserType.USER;
                byte[] salt = Encoding.UTF8.GetBytes(newUser.Username + newUser.Email + newUser.AccountCreated.ToString() + _configuration["SALT"]);
                newUser.Password = Codec.GenerateHash(Encoding.UTF8.GetString(Convert.FromBase64String(user.Password)), salt);
                bool userRegistered = _agent.AddEntity(newUser);

                return userRegistered ? Ok(_mapper.Map<UserDTO>(newUser)) : BadRequest("Failed to create user.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create user. Error: {ex}");
                return BadRequest($"Failed to create user due to an unknown reason.");
            }
        }
    }
}
