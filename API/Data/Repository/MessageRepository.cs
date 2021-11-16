using API.Entities;
using API.Helpers;
using API.Interfaces.Repository;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<MessageModel>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.DateRead == null && u.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageModel>(_mapper.ConfigurationProvider);

            return await PagedList<MessageModel>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<PagedList<MessageModel>> GetMessageThread(MessageParams messageParams)
        {
            var query = _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.Recipient.UserName == messageParams.Username
                         && m.RecipientDeleted == false
                         && m.Sender.UserName == messageParams.RecipientUsername
                         || m.Recipient.UserName == messageParams.RecipientUsername
                         && m.Sender.UserName == messageParams.Username
                         && m.SenderDeleted == false)
                .AsQueryable();

            var unreadMessages = query.Where(m => m.DateRead == null && m.Recipient.UserName == messageParams.Username).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            var messages = query.ProjectTo<MessageModel>(_mapper.ConfigurationProvider);

            return await PagedList<MessageModel>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
