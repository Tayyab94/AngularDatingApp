using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.ImplementedRepositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
               context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            // return await context.Messages.FindAsync(id);
              return await context.Messages.Include(s=>s.Sender).Include(s=>s.Receiver)
                    .FirstOrDefaultAsync(s=>s.Id==id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query= context.Messages.OrderByDescending(s=>s.MessageSent)
            .AsQueryable();

            query= messageParams.Container switch
            {
                "Inbox"=> query.Where(s=>s.Receiver.Username == messageParams.Username),
                "Outbox" => query.Where(s=>s.Sender.Username== messageParams.Username),
                _ => query.Where(s => s.Receiver.Username == messageParams.Username && s.DataRead== null)
            };

            var message= query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(message,messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string receiverUserName)
        {
            var messages= await context.Messages
                        .Include(s=>s.Sender).ThenInclude(p=>p.Photos)
                        .Include(s=>s.Receiver).ThenInclude(s=>s.Photos)
                        .Where(s=>(s.Receiver.Username == currentUsername
                                && s.Sender.Username == receiverUserName)
                                || (s.Receiver.Username == receiverUserName
                                && s.Sender.Username== currentUsername))
                        .OrderBy(s=>s.MessageSent)
                        .ToListAsync();
            
            var unreadMessage= messages.Where(s=>s.DataRead== null 
             && s.Receiver.Username == currentUsername).ToList();

             if(unreadMessage.Any())
             {
                 foreach (var unread in unreadMessage)
                 {
                     unread.DataRead= DateTime.Now;
                 }
                 await context.SaveChangesAsync();
             }
             return mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsycn()
        {
            return await context.SaveChangesAsync()>0;
        }
    }
}