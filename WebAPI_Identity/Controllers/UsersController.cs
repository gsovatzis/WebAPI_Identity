using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI_Identity.DTOs;
using WebAPI_Identity.Helpers;
using WebAPI_Identity.Models;
using WebAPI_Identity.Services;

namespace WebAPI_Identity.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;

        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserDTO UserDTO)
        {
            var user = _userService.Authenticate(UserDTO.UserName, UserDTO.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDTO UserDTO)
        {
            // map dto to entity
            var user = _mapper.Map<MyUser>(UserDTO);

            try
            {
                // save 
                var result = _userService.Create(user, UserDTO.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var UserDTOs = _mapper.Map<IList<UserDTO>>(users);
            return Ok(UserDTOs);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var user = _userService.GetById(id);
            var UserDTO = _mapper.Map<UserDTO>(user);
            return Ok(UserDTO);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]UserDTO UserDTO)
        {
            // map dto to entity and set id
            var user = _mapper.Map<MyUser>(UserDTO);
            user.Id = id;

            try
            {
                // save 
                _userService.Update(user, UserDTO.CurrentPass, UserDTO.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }

}