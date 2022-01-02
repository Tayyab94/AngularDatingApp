using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.ImplementedRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberByIdAsync(int id)
        {
              return await context.AppUsers.Where(s=>s.Id== id)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<MemberDTO> GetMemberByUsernameAsync(string userName)
        {
            return await context.AppUsers.Where(s=>s.Username== userName)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await context.AppUsers
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await context.AppUsers.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string userName)
        {
            return await context.AppUsers.Include(s=>s.Photos).SingleOrDefaultAsync(s=>s.Username == userName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.AppUsers.Include(s=>s.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync()>0;
        }

        public void UpdateUser(AppUser user)
        {
            context.Entry(user).State= EntityState.Modified;
        }
    }
}