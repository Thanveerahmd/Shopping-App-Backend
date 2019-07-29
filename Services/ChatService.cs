using pro.backend.Entities;
using Project.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pro.backend.iServices;
using System;

namespace pro.backend.Services
{
    public class ChatService : iChatService
    {
        private DataContext _context;

        public ChatService(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Chat>> GetAllChatsOfAdminFromUser(string userId)
        {

            var info = await _context.Chat
            .Where(p => (p.Receiver == "admin" && p.Sender == userId) || (p.Receiver == userId && p.Sender == "admin"))
            .ToListAsync();

            return info;
        }

        public async Task<ICollection<Chat>> GetLastMessageFromUsers()
        {

            var info = await _context.Chat
            .Where(p => p.Receiver == "admin" || p.Sender == "admin")
            .ToListAsync();

            return info;
        }

        public async Task<bool> GetUnreadBoolForAdmin()
        {

            var info = await _context.Chat
            .Where(p => p.Receiver == "admin")
            .Where(p => p.isUnRead == true)
            .ToListAsync();

            if (info != null)
                return true;
            else
                return false;
        }

        public async Task<bool> UpdateAllChatsOfAdminFromUserToRead(string userId)
        {

            var info = await _context.Chat
            .Where(p => (p.Receiver == "admin" && p.Sender == userId))
            .ToListAsync();

            try
            {
                foreach (var item in info)
                {
                    item.isUnRead = false;
                    _context.Chat.Update(item);
                }
                return true;
            }
            catch (AppException e)
            {
                Console.WriteLine(e);
                return false;
            }

        }
    }
}