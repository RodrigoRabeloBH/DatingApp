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
    public class MessagesController : BaseController
    {
        private readonly IMessageServices _services;

        public MessagesController(IMessageServices services)
        {
            _services = services;
        }

        [HttpPost]
        public async Task<ActionResult<MessageModel>> CreateMessage(CreateMessageModel model)
        {
            var username = User.GetUsername();

            var messageModel = await _services.CreateMessage(username, model);

            if (messageModel != null)
            {
                return Ok(messageModel);
            }
            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageModel>>> GetMessages([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _services.GetMessages(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpGet("thread")]
        public async Task<ActionResult<IEnumerable<MessageModel>>> GetMessageThread([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _services.GetMessageThread(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var wasDeleted = await _services.DeleteMessage(username, id);

            if (wasDeleted) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}
