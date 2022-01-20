using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Repositories
{
    public interface ILikedRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likeUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<PagedList<LikeDTO>>GetUserLikes(LikesParams likesParams);

        Task<bool> AddLike(UserLike userLike);
        
    }
}