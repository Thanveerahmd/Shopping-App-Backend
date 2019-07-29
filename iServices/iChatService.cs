using pro.backend.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pro.backend.iServices
{
    public interface iChatService
    {

        Task<ICollection<Chat>> GetAllChatsOfAdminFromUser(string userId);

        Task<IEnumerable<Chat>> GetLastMessageFromUsers();

        Task<bool> GetUnreadBoolForAdmin();
        Task<Chat> GetChat(int ChatId);
        Task<bool> UpdateAllChatsOfAdminFromUserToRead(string userId);

        Task<ICollection<Chat>> GetAllChatsOfUserFromAdmin(string userId);

        Task<bool> GetUnreadBoolForUser(string userId);

        Task<bool> UpdateAllChatsOfUserFromAdminToRead(string userId);
    }
}