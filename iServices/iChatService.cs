using pro.backend.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pro.backend.iServices
{
    public interface iChatService
    {
        
        Task<ICollection<Chat>> GetAllChatsOfAdminFromUser(string userId);

       Task<ICollection<Chat>> GetLastMessageFromUsers();

       Task<bool> GetUnreadBoolForAdmin();

       Task<bool> UpdateAllChatsOfAdminFromUserToRead(string userId);
    }
}