using API.Entities;
using API.Helpers;
using API.Interfaces.Repository;
using API.Interfaces.Services;
using API.Models;
using AutoMapper;
using System.Threading.Tasks;

namespace API.Services
{
    public class MessageServices : IMessageServices
    {
        private readonly IUserRepository _userRepository;

        private readonly IMessageRepository _messageRepository;

        private readonly IMapper _mapper;

        public MessageServices(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<MessageModel> CreateMessage(string username, CreateMessageModel model)
        {
            if (username == model.RecipientUsername.ToUpper()) return null;

            var sender = await _userRepository.GetUserByUsernameAsync(username);

            var recipient = await _userRepository.GetUserByUsernameAsync(model.RecipientUsername.ToUpper());

            if (recipient == null) return null;

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = model.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                return _mapper.Map<MessageModel>(message);
            }

            return null;
        }

        public async Task<PagedList<MessageModel>> GetMessages(MessageParams messageParams)
        {
            var messages = await _messageRepository.GetMessageForUser(messageParams);

            return messages;
        }

        public async Task<PagedList<MessageModel>> GetMessageThread(MessageParams messageParams)
        {
            var messages = await _messageRepository.GetMessageThread(messageParams);

            return messages;
        }

        public async Task<bool> DeleteMessage(string username, int messageId)
        {
            var message = await _messageRepository.GetMessage(messageId);

            if (message.Sender.UserName != username && message.Recipient.UserName != username) return false;

            if (message.Sender.UserName == username) message.SenderDeleted = true;

            if (message.Recipient.UserName == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) _messageRepository.DeleteMessage(message);

            return await _messageRepository.SaveAllAsync();
        }
    }
}
