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
        public async Task<Chat> GetChat(int ChatId)
        {
          var info =await _context.Chat.FindAsync(ChatId);

            return info;
        }

        public async Task<ICollection<Chat>> GetLastMessageFromUsers()
        {
            var chats = await _context.Chat.ToListAsync();

            IList<Chat> lastChats = new List<Chat>();

            foreach (var item in chats)
            {
                if (!item.Sender.Equals("admin"))
                {
                    var info = await _context.Chat
                                .Where(p => p.Receiver == "admin").
                                LastOrDefaultAsync(p => p.Sender == item.Sender);

                    if (!lastChats.Contains(info))
                        lastChats.Add(info);
                }
            }
            return lastChats;
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

        public async Task<IEnumerable<Chat>> GetAllChatsOfUserFromAdmin(string userId)
        {

            var info = await _context.Chat
            .Where(p => (p.Receiver == "admin" && p.Sender == userId) || (p.Receiver == userId && p.Sender == "admin"))
            .ToListAsync();

            var data = info.OrderByDescending(p => p.Id);
            return data;
        }

        public async Task<bool> GetUnreadBoolForUser(string userId)
        {

            var info = await _context.Chat
            .Where(p => p.Receiver == userId)
            .Where(p => p.isUnRead == true)
            .ToListAsync();

            if (info != null)
                return true;
            else
                return false;
        }

        public async Task<bool> UpdateAllChatsOfUserFromAdminToRead(string userId)
        {

            var info = await _context.Chat
            .Where(p => (p.Receiver == userId && p.Sender == "admin"))
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