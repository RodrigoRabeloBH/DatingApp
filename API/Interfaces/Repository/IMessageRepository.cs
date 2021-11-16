using API.Entities;
using API.Helpers;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces.Repository
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);
        Task<PagedList<MessageModel>> GetMessageForUser(MessageParams messageParams);
        Task<PagedList<MessageModel>> GetMessageThread(MessageParams messageParams);
        Task<bool> SaveAllAsync();
    }
}
