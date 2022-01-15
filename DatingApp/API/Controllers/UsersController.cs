using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper,
            IPhotoService photoService)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.photoService = photoService;
            // this._context = context;
        }

        [HttpGet]
        // [AllowAnonymous]

         [Authorize]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery]UserParams userParam)
        {
                 //var users=await userRepository.GetUsersAsync();
        
            // Sotring the Users
             var user = await userRepository.GetMemberByUsernameAsync(User.GetUserName());
            userParam.CurentUserName= user.Username;
            if(string.IsNullOrEmpty(userParam.Gender))
            {
                userParam.Gender = user.Gender =="male"? "female":"male";
            }

            var users=await userRepository.GetMembersAsync(userParam);

            // var returnUsers = mapper.Map<IEnumerable<MemberDTO>>(users);

            Response.AddPaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);
            return Ok(users);
        }


        // [HttpGet("{id}")]
        // [Authorize]
        // public async Task<ActionResult<AppUser>> GetUser(int id)
        // {
        //     var user =await this._context.AppUsers.FindAsync(id);
        //     return user;
        // }

            [HttpGet("{username}", Name ="GetUser")]
        [Authorize]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var user =mapper.Map<MemberDTO>(await this.userRepository.GetUserByUsernameAsync(username));
            return user;
        }

        [HttpPut]

        public async Task<ActionResult>UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            // var userName=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // var user= await userRepository.GetUserByUsernameAsync(userName);


            var user= await userRepository.GetUserByUsernameAsync(User.GetUserName());

            mapper.Map(memberUpdateDTO, user);

           userRepository.UpdateUser(user);
           
           if(await userRepository.SaveAllAsync()) return NoContent();
          return BadRequest("Faild to Update User");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user= await userRepository.GetUserByUsernameAsync(User.GetUserName());

            var result= await photoService.AddPhotoAsync(file);

            if(result.Error!=null) return BadRequest(result.Error.Message);

            var photo= new Photo
            {
                Url= result.SecureUrl.ToString(),
                PublicId= result.PublicId
            };
            
            if(user.Photos.Count==0)
            {
                photo.IsMain=true;
            }

            user.Photos.Add(photo);

            if(await userRepository.SaveAllAsync())
                {
                    // return mapper.Map<PhotoDTO>(photo);

                    return CreatedAtRoute("GetUser",new {username= User.GetUserName()},mapper.Map<PhotoDTO>(photo));
                }

                return BadRequest("Problem adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]

        public async Task<ActionResult>SetMainPhoto(int photoId)
        {
            var user = await userRepository.GetMemberByUsernameAsync(User.GetUserName());
            
            var photo= user.Photos.FirstOrDefault(s=> s.Id == photoId);

            if(photo.IsMain) return BadRequest("This is aleady main Photo");
            
            var currentMain = user.Photos.FirstOrDefault(s=>s.IsMain);

            if(currentMain!=null) currentMain.IsMain=false;

            photo.IsMain=true;

    
            try
            {
                // userRepository.updateUserPhoto(photo.Id,photoId);
                  if(await userRepository.SaveAllAsync())
                     {
                    return NoContent();
                    } 
            }
            catch (System.Exception e)
            {
                
                throw;
            }
           
            return BadRequest("Fail to set main Photo");
        }

        [HttpDelete("delete-photo/{photoId}")]

        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userRepository.GetMemberByUsernameAsync(User.GetUserName());

            var photo= user.Photos.FirstOrDefault(s=>s.Id ==photoId);

            if(photo == null) return NotFound();

            // if(photo.IsMain) return BadRequest("You can not delte main Photo");

            if(photo.PublicId!= null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error!= null) return BadRequest(result.Error.Message);
            }


            user.Photos.Remove(photo);

            userRepository.deletePhotoFromDb(photo.Id);
            return Ok();
            // if(await userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Fail to delete");
        }
    }
}