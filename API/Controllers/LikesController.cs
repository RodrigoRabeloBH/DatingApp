using API.Extensions;
using API.Helpers;
using API.Interfaces.Services;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseController
    {
        private readonly ILikeServices _services;

        public LikesController(ILikeServices services)
        {
            _services = services;
        }

        [HttpPost("{likedUserName}")]
        public async Task<ActionResult> AddLike(string likedUserName)
        {
            int sourceUserId = User.GetUserId();

            await _services.AddLike(sourceUserId, likedUserName.ToUpper());

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeModel>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();

            var users = await _services.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}
