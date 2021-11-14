using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Interfaces.Repository;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _rep;

        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository rep, IMapper mapper, IPhotoService photoService)
        {
            _rep = rep;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get([FromQuery] UserParams userParams)
        {
            var currentUser = await _rep.GetUserByUsernameAsync(User.GetUsername());

            userParams.CurrentUsername = currentUser.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender.ToLower() == "male" ? "female" : "male";
            }

            var users = await _rep.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetByUsername(string username)
        {
            var user = await _rep.GetMemberAsync(username);

            if (user == null) return NotFound(new ApiResponse(404));

            return user;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(MemberUpdateDto model)
        {
            var user = await _rep.GetUserByUsernameAsync(User.GetUsername());

            _mapper.Map(model, user);

            await _rep.Update(user);

            return NoContent();
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _rep.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(new ApiResponse(400, result.Error.Message));

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            await _rep.Update(user);

            return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _rep.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

            if (currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            await _rep.Update(user);

            return NoContent();
        }

        [HttpDelete("remove-photo/{photoId}")]
        public async Task<ActionResult> RemovePhoto(int photoId)
        {
            var user = await _rep.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            await _rep.Update(user);

            return Ok();
        }
    }
}