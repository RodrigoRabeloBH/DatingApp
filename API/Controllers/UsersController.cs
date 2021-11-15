using System.Threading.Tasks;
using API.Extensions;
using API.Helpers;
using API.Interfaces.Services;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserServices _service;
        public UsersController(IUserServices service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get([FromQuery] UserParams userParams)
        {
            string username = User.GetUsername();

            var users = await _service.GetAllMembers(userParams, username);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberModel>> GetByUsername(string username)
        {
            var user = await _service.GetByUsername(username);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(MemberUpdateModel model)
        {
            string username = User.GetUsername();

            var wasUpdated = await _service.UpdateUser(username, model);

            if (wasUpdated) return NoContent();

            return BadRequest();
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoModel>> AddPhoto(IFormFile file)
        {
            string username = User.GetUsername();

            var photoModel = await _service.AddPhoto(username, file);

            if (photoModel != null)
            {
                return CreatedAtRoute("GetUser", new { username }, photoModel);
            }

            return BadRequest();
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            string username = User.GetUsername();

            var waSeted = await _service.SetMainPhoto(username, photoId);

            if (waSeted) return NoContent();

            return BadRequest();
        }

        [HttpDelete]
        public async Task<ActionResult> Remove()
        {
            string username = User.GetUsername();

            bool wasRemoved = await _service.RemoveUser(username);

            if (wasRemoved) return Ok();

            return BadRequest();
        }

        [HttpDelete("remove-photo/{photoId}")]
        public async Task<ActionResult> RemovePhoto(int photoId)
        {
            string username = User.GetUsername();

            bool wasRemoved = await _service.RemovePhoto(username, photoId);

            if (wasRemoved) return Ok();

            return BadRequest();
        }
    }
}