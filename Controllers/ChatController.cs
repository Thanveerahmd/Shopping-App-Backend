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

        [HttpPost("admin/{userEmail}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessageFromAdmin(ChatDto chat,string userEmail){

            var message = _mapper.Map<Chat>(chat);
            message.Sender = "admin";
            var userId = await _usermanger.FindByEmailAsync(chat.ReceiverEmail);
            message.Receiver = userId.Id;
            _repo.Add(message);
            return Ok();
        }

        [HttpGet("admin/{userEmail}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMessageForAdminFromUser(string userEmail){

            var userId = await _usermanger.FindByEmailAsync(userEmail);
            var chats = await _chatService.GetAllChatsOfAdminFromUser(userId.Id);
            var chatsinfo = _mapper.Map<IEnumerable<Chat>>(chats);
            return Ok(chatsinfo);
        }

        [HttpGet("admin/all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLastMessageFromUsers(){

            
            var chats = await _chatService.GetLastMessageFromUsers();
            var chatsinfo = _mapper.Map<IEnumerable<Chat>>(chats);
            return Ok(chatsinfo);
        }

        [HttpGet("admin/unread")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUnreadMessageForAdmin(){

            var chatsBool = await _chatService.GetUnreadBoolForAdmin();
            return Ok(chatsBool);
        }

        [HttpPost("admin/opened/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> MakeUnreadMessagesAsReadAdmin(string userId){

            var chatsBool = await _chatService.UpdateAllChatsOfAdminFromUserToRead(userId);
            return Ok(chatsBool);
        }


        [HttpPost("user")]
        [AllowAnonymous]
        public IActionResult SendMessageFromUser(ChatDto chat){

            var message = _mapper.Map<Chat>(chat);
            message.Receiver = "admin";
            _repo.Add(message);
            return Ok();
        }

        [HttpGet("user/unread/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUnreadMessageForUser(string userId){

            var chatsBool = await _chatService.GetUnreadBoolForUser(userId);
            return Ok(chatsBool);            
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMessageForUserFromAdmin(string userId){

            var messages = await _chatService.GetAllChatsOfUserFromAdmin(userId);
            return Ok(messages);
        }

        [HttpPost("user/opened/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> MakeUnreadMessagesAsReadUser(string userId){

            var chatsBool = await _chatService.UpdateAllChatsOfUserFromAdminToRead(userId);
            return Ok(chatsBool);
        }
    }
}