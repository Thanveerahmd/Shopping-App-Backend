using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.Helpers;
using pro.backend.iServices;
using Project.Entities;


namespace pro.backend.Controllers
{
    [ApiController]
    [Route("chat")]

    public class ChatController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _usermanger;
        private readonly iChatService _chatService;
        public readonly iShoppingRepo _repo;

        public ChatController(
            IMapper mapper,
            iChatService chatService,
            iShoppingRepo repo,
            UserManager<User> usermanger
            )
        {

            _mapper = mapper;
            _chatService = chatService;
            _repo = repo;
            _usermanger = usermanger;

        }

        [HttpPost("admin")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessageFromAdmin(ChatDto chat)
        {

            var message = _mapper.Map<Chat>(chat);
            message.Sender = "admin";
            var userId = await _usermanger.FindByEmailAsync(chat.ReceiverEmail);
            message.Receiver = userId.Id;
            message.isUnRead = true;
            _repo.Add(message);
            if (await _repo.SaveAll())
            {
                return Ok(message);
            }
            return BadRequest();
        }

        [HttpDelete("{ChatId}/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Deleteinfo(int ChatId, string userId)
        {
            var chat = await _chatService.GetChat(ChatId);
            TimeSpan duration = DateTime.Now - chat.TimeSent;
            if (!chat.Sender.Equals("admin"))
            {
                if (chat.Sender.Equals(userId) && (duration.TotalMinutes <= 10))
                {
                    _repo.Delete(chat);
                    if (await _repo.SaveAll())
                        return Ok();
                    return BadRequest();
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpGet("admin/{userEmail}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMessageForAdminFromUser(string userEmail)
        {

            var userId = await _usermanger.FindByEmailAsync(userEmail);
            var chats = await _chatService.GetAllChatsOfAdminFromUser(userId.Id);
            var chatsinfo = _mapper.Map<IEnumerable<Chat>>(chats);
            return Ok(chatsinfo);
        }

        [HttpGet("admin/all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLastMessageFromUsers()
        {
            var chats = await _chatService.GetLastMessageFromUsers();
            var chatsinfo = _mapper.Map<IEnumerable<ChatDto>>(chats);
            IList<ChatDto> lastChats = new List<ChatDto>();
            foreach (var item in chatsinfo)
            {
                if (!item.Sender.Equals("admin"))
                {
                    var user = await _usermanger.FindByIdAsync(item.Sender);
                    item.UserMail = user.Email;
                    item.UserFullName = user.FirstName + " " + user.LastName;
                    lastChats.Add(item);
                }
            }
            return Ok(lastChats);
        }

        [HttpGet("admin/unread")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUnreadMessageForAdmin()
        {

            var chatsBool = await _chatService.GetUnreadBoolForAdmin();
            return Ok(chatsBool);
        }

        [HttpPost("admin/opened/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> MakeUnreadMessagesAsReadAdmin(string userId)
        {

            var chatsBool = await _chatService.UpdateAllChatsOfAdminFromUserToRead(userId);
            return Ok(chatsBool);
        }


        [HttpPost("user")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessageFromUser(ChatDto chat)
        {

            var message = _mapper.Map<Chat>(chat);
            message.Receiver = "admin";
            message.isUnRead = true;
            _repo.Add(message);
            if (await _repo.SaveAll())
            {
                return Ok(message.Id);
            }
            return BadRequest();
        }

        [HttpGet("user/unread/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUnreadMessageForUser(string userId)
        {

            var chatsBool = await _chatService.GetUnreadBoolForUser(userId);
            return Ok(chatsBool);
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMessageForUserFromAdmin(string userId)
        {

            var messages = await _chatService.GetAllChatsOfUserFromAdmin(userId);
            return Ok(messages);
        }

        [HttpPost("user/opened/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> MakeUnreadMessagesAsReadUser(string userId)
        {

            var chatsBool = await _chatService.UpdateAllChatsOfUserFromAdminToRead(userId);
            return Ok(chatsBool);
        }
    }
}