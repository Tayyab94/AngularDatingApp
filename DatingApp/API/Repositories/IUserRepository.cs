using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Repositories
{
    public interface IUserRepository
    {
        void UpdateUser(AppUser user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>>GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string userName);
        
    }
}