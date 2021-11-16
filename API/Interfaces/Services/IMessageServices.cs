using API.Helpers;
using API.Models;
using System.Threading.Tasks;

namespace API.Interfaces.Services
{
    public interface IMessageServices
    {
        Task<MessageModel> CreateMessage(string username, CreateMessageModel model);
        Task<PagedList<MessageModel>> GetMessages(MessageParams messageParams);
        Task<PagedList<MessageModel>> GetMessageThread(MessageParams messageParams);
        Task<bool> DeleteMessage(string username, int messageId);
    }
}
