using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMoviesAPI.Data;
using MyMoviesAPI.Data.Entities;
using MyMoviesAPI.Models;
using MyMoviesAPI.Models.User;
using MyMoviesAPI.Utility;

namespace MyMoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IDatabaseAgent _agent;
        private readonly ILogger<MovieController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private string forgotPasswordURL = "http://localhost:3000/forgot-password";

        public UserController(IDatabaseAgent agent, ILogger<MovieController> logger, IMapper mapper, IConfiguration configuration)
        {
            _agent = agent;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        // POST: api/User/SendResetPasswordLink?email={email}
        [HttpPost("SendResetPasswordLink")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult PutSendResetPassworLink([FromQuery] string email)
        {
            try
            {
                // Check if a user exists with the email address
                var user = _agent.GetUserByEmail(email);

                if (user != null)
                {
                    string requestID = Guid.NewGuid().ToString();

                    // Save password reset to database
                    PasswordResetRequest req = new PasswordResetRequest();
                    req.UserID = user.ID;
                    req.RequestDate = DateTime.Now;
                    req.ID = Codec.GenerateHash(requestID, Encoding.UTF8.GetBytes(_configuration["SALT"]));
                    _agent.AddEntity(req);

                    // Send email with the link to change the password
                    MailSender.SendPasswordResetMail(email, forgotPasswordURL + "?id=" + requestID);
                }

                return Ok("The password reset link was sent to the provided email address if the user exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send the email for password reset. Error: {ex}");
                return BadRequest($"Failed to send the email for password reset due to an unknown reason.");
            } 
        }

        // PUT: api/User/ResetPassword?id={id}&password=${password}
        [HttpPut("ResetPassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult PutResetPassword([FromQuery] string id, [FromQuery] string password)
        {
            try
            {
                // Check if database contains the hash value of the provided id
                string hashedId = Codec.GenerateHash(id, Encoding.UTF8.GetBytes(_configuration["SALT"]));
                var forgotPasswordRequestsFromDB = _agent.GetPasswordResetRequests(hashedId);

                if(forgotPasswordRequestsFromDB != null && forgotPasswordRequestsFromDB.Count > 0)
                {
                    var latestRequest = forgotPasswordRequestsFromDB.OrderByDescending(req => req.RequestDate).FirstOrDefault();

                    // Confirm that less than 8 hours has passed
                    var timeDifferenceInHours = (DateTime.Now - latestRequest.RequestDate).TotalHours;
                    if(timeDifferenceInHours < 8)
                    {
                        // Update users password
                        var userFromDB = _agent.GetUserById(latestRequest.UserID);

                        if(userFromDB != null)
                        {
                            User updatedUser = userFromDB;
                            updatedUser.Password = Codec.GenerateHash(Encoding.UTF8.GetString(Convert.FromBase64String(password)), Encoding.UTF8.GetBytes(_configuration["SALT"]));
                            return _agent.UpdateUser(updatedUser) ? Ok("Password successfully updated.") : BadRequest("Failed to update password.");
                        }
                    } 
                    else
                    {
                        return BadRequest("The forgot password request has expired, please fill in another one to reset your password.");
                    }
                }
                return BadRequest("Failed to reset password. Try to send another forgot password request.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send the email for password reset. Error: {ex}");
                return BadRequest($"Failed to send the email for password reset due to an unknown reason.");
            }
        }

        // PUT: api/User/updateuser
        [HttpPut("updateuser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<UserDTO> PutUpdateUser(UserUpdateDTO user)
        {
            try
            {
                // Check if a user exists with the email address
                var userFromDB = _agent.GetUserById(user.ID);

                if (userFromDB != null && user.IsUserValid())
                {
                    // Confirm that the username is not taken
                    if(userFromDB.Username != user.Username && user.IsUsernameValid())
                    {
                        // Try to find a user with the new username
                        var userFromDBWithUsername = _agent.GetUserByUsername(user.Username);
                        if (userFromDBWithUsername != null) return BadRequest("Username is already taken, please pick another one.");
                    }

                    // Confirm that the email is not taken
                    if(userFromDB.Email != user.Email && user.IsEmailValid())
                    {
                        // Try to find a user with the new email
                        var userFromDBWithEmail = _agent.GetUserByEmail(user.Email);
                        if (userFromDBWithEmail != null) return BadRequest("Email is already taken, please pick another one.");
                    }

                    userFromDB.Email = user.IsEmailValid() ? user.Email : userFromDB.Email;
                    userFromDB.Username = user.IsUsernameValid() ? user.Username : userFromDB.Username;
                    return _agent.UpdateUser(userFromDB) ? Ok(_mapper.Map<UserDTO>(userFromDB)) : BadRequest("Failed to update user data.");

                }

                return BadRequest("Failed to update user data");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to udpate user data. Error: {ex}");
                return BadRequest($"Failed to update user data due to an unknown reason.");
            }
        }
    }
}
