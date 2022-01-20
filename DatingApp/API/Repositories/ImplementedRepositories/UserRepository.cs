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

        
        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParam)
        {
            // var query= context.AppUsers
            //     .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            //     .AsNoTracking().AsQueryable();

            // return await PagedList<MemberDTO>.CreateAsync(query,userParam.PageNumber,userParam.PageSize);

            var query= context.AppUsers.AsQueryable();

            query = query.Where(s=>s.Username != userParam.CurentUserName);
            query = query.Where(s=>s.Gender== userParam.Gender);

            var minDOB = DateTime.Today.AddYears(-userParam.MaxAge -1);
            var maxDOB = DateTime.Today.AddYears(-userParam.MinAge);

            query = query.Where(s=>s.DateOfBirth >= minDOB && s.DateOfBirth <=maxDOB);

            // apply the switch statement 
            query = userParam.OrderBy switch
            {
                "created"=> query.OrderByDescending(s=>s.Created),
                _=> query.OrderByDescending(s=>s.LastActive)
            };
            
            return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(
                                            mapper.ConfigurationProvider).AsNoTracking(),
                                            userParam.PageNumber,userParam.PageSize);

        }
        public async Task<Photo> GetPhoto(int photoId)
        {   
           return await context.Photos.FirstOrDefaultAsync(s=>s.Id==photoId);
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

        public void updateUserPhoto(int UserPhotoId,int photoId)
        {
            var data = context.Photos.Where(s=>s.Id== photoId).FirstOrDefault();
            data.IsMain= false;
            context.Entry(data).State= EntityState.Modified;
            var photo = context.Photos.Where(s=>s.Id== photoId).FirstOrDefault();
            data.IsMain= true;
            context.Entry(data).State= EntityState.Modified;

        }

        
       public void deletePhotoFromDb(int id)
        {
            var data= context.Photos.Where(s=>s.Id==id).FirstOrDefault();
            context.Photos.Remove(data);
            context.SaveChanges();
        }
    }
}