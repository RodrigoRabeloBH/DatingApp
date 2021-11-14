using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountServices _services;

        private readonly IMapper _mapper;

        public AccountController(IAccountServices services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = await _services.UserExists(model.Username);

            if (user != null) return BadRequest(new ApiResponse(400, "Username is taken"));

            bool wasCreated = await _services.Register(model);

            if (wasCreated)
            {
                var newUser = _mapper.Map<AppUser>(model);

                var userModel = new UserModel
                {
                    Username = newUser.UserName,
                    Token = _services.CreateToken(newUser),
                    KnownAs = newUser.KnownAs,
                    PhotoUrl = newUser.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                    Gender = user.Gender
                };

                return StatusCode(201, userModel);
            }

            return BadRequest(new ApiResponse(400));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _services.UserExists(model.Username);

            if (user == null) return Unauthorized(new ApiResponse(401));

            if (!await _services.Login(user, model)) return Unauthorized(new ApiResponse(401));

            var userModel = new UserModel
            {
                Username = user.UserName,
                Token = _services.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

            return StatusCode(200, userModel);
        }
    }
}