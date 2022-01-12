using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenServices _tokenServices;
        private readonly IMapper mapper;

        public AccountController(DataContext context, ITokenServices tokenServices,
         IMapper mapper)
        {
            this.context = context;
            this._tokenServices = tokenServices;
            this.mapper = mapper;
        }        


        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegistrationDTO registrationDTO)
        {
            if(await UserExist(registrationDTO.username)) return BadRequest("User-Name is Already Taken");
            
            var user = mapper.Map<AppUser>(registrationDTO);
            using var hmac=  new HMACSHA512();
           
                user.Username= registrationDTO.username;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationDTO.password));
                user.PasswordSalt= hmac.Key;
            
            this.context.AppUsers.Add(user);
           await this.context.SaveChangesAsync();

           return new UserDTO(){
               userName = user.Username,
                Token= _tokenServices.CreateToken(user),
                photoUrl= user.Photos.FirstOrDefault(s=>s.IsMain).Url,
                 knownAs = user.KnownAs
           };
        }


        private  async Task<bool> UserExist(string userName)
        {
            return await this.context.AppUsers.AnyAsync(u=>u.Username.ToLower()== userName.ToLower());
        }


        [HttpPost("Login")]

        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user =await this.context.AppUsers.Include(s=>s.Photos).SingleOrDefaultAsync(s=>s.Username== loginDTO.username);

            if(user== null) return Unauthorized("Invalud UserName");

            using var hmac= new HMACSHA512(user.PasswordSalt);

            var computedHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

            for (int i = 0; i < computedHash.Length; i++)
            {   
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDTO(){userName = user.Username, Token = _tokenServices.CreateToken(user),
                photoUrl = user.Photos.FirstOrDefault()?.Url, knownAs = user.KnownAs};
        }
    }
}