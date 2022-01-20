using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.ImplementedRepositories
{
    public class LikedRepository : ILikedRepository
    {
        private readonly DataContext _context;

        public LikedRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<bool> AddLike(UserLike userLike)
        {
            try
            {
                 _context.UserLikes.Add(userLike);

                await  _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                return false;
            }
           
           return true;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {
            return await _context.UserLikes.FindAsync(sourceUserId,likeUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams lPrams)
        {
            var users= _context.AppUsers.OrderBy(s=>s.Username).AsQueryable();

            var likes =_context.UserLikes.AsQueryable();

            if(lPrams.Predicate =="liked")
            {
                likes= likes.Where(s=>s.SourceUserId== lPrams.UserId);
            
                users= likes.Select(like=>like.LikedUser);
            }

             if(lPrams.Predicate =="likedBy")
            {
                likes= likes.Where(s=>s.LikeUserId== lPrams.UserId);

                users= likes.Select(like=>like.SorceUser);
            }

            var likedUser= users.Select(u=>new LikeDTO(){
                     Username= u.Username,
                    Age= u.DateOfBirth.CalculateAge(),
                     PhotoUrl =u.Photos.FirstOrDefault(s=>s.IsMain).Url,
                    knownAs= u.KnownAs,
                     City= u.City,
                      Id= u.Id, 
            });

            return await PagedList<LikeDTO>.CreateAsync(likedUser,lPrams.PageNumber,lPrams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
          return await _context.AppUsers.Include(s=>s.LikedUser).Where(s=>s.Id== userId)
          .FirstOrDefaultAsync();
        }
    }
}