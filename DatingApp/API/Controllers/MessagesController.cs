
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {

        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(IUserRepository userRepository,
            IMessageRepository messageRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessage)
        {
            var userName = User.GetUserName();
            if (userName == createMessage.RecipientUsername.ToLower())
                return BadRequest("You can Not send message to yourself");

            var sender = await userRepository.GetUserByUsernameAsync(userName);
            var receiver = await userRepository.GetUserByUsernameAsync(createMessage.RecipientUsername);

            if (receiver == null)
            {
                return NotFound("Receiver Not found");
            }

            var message = new Message()
            {
                Sender = sender,
                Receiver = receiver,
                Content = createMessage.Content,
                SenderName = sender.Username,
                ReceiverName = receiver.Username
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsycn()) return Ok(mapper.Map<MessageDTO>(message));


            return BadRequest("Fail to send Message");
        }

        [HttpGet]

        public async Task<IEnumerable<MessageDTO>> GetMessageForUser
          ([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();

            var message = await messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(message.CurrentPage, message.PageSize, message.TotalCount, message.TotalPages);

            return message;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
            var currentUserName = User.GetUserName();

            return Ok(await messageRepository.GetMessageThread(currentUserName, username));
        }
        

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
                var userName= User.GetUserName();

                var message= await messageRepository.GetMessage(id);

                if(message.Sender.Username!= userName && message.Receiver.Username!= userName)
                {
                    return Unauthorized();
                }

                if(message.Sender.Username== userName) message.SenderDelated=true;

                if(message.Receiver.Username == userName) message.ReceriverDeleted=true;

                if(message.SenderDelated && message.ReceriverDeleted)
                {
                     messageRepository.DeleteMessage(message);
                }

                if(await messageRepository.SaveAllAsycn())  return Ok();

                return BadRequest("Problem Deleting the Message");
        }
    }
}