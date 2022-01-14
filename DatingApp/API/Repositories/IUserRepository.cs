using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Repositories
{
    public interface IUserRepository
    {
        void UpdateUser(AppUser user);

        Task<Photo>GetPhoto(int photoId);
         void updateUserPhoto(int UserPhotoId, int photoID);
        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>>GetUsersAsync();


        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string userName);

        
        Task<PagedList<MemberDTO>>GetMembersAsync(UserParams userParam);

        
        Task<MemberDTO> GetMemberByIdAsync(int id);
        Task<MemberDTO> GetMemberByUsernameAsync(string userName);

        void deletePhotoFromDb(int id);
        
    }
}