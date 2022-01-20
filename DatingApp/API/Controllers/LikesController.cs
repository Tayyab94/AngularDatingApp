using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]

    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        private readonly ILikedRepository _likedRepository;

        public LikesController(IUserRepository userRepository,ILikedRepository likedRepository )
        {
                this._userRepository = userRepository;
                this._likedRepository = likedRepository;
        }
        

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId= User.GetUserId();
            var likedUser= await _userRepository.GetUserByUsernameAsync(username);
            
            var sourceUser = await _likedRepository.GetUserWithLikes(sourceUserId);

            if(likedUser== null)
            {
                return NotFound();
            }

            if(sourceUser.Username == username) return BadRequest("You can not like yourself");

            var userLike = await _likedRepository.GetUserLike(sourceUserId,likedUser.Id);

            if(userLike != null) return BadRequest("You already like this User");
            
            userLike= new UserLike
            {
                 SourceUserId= sourceUserId,
                  LikeUserId= likedUser.Id
            };

           if(await _likedRepository.AddLike(userLike)) return Ok();
            // if(await _userRepository.SaveAllAsync()) return Ok();


            return BadRequest("Fail to Like User");
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes(string predicate)
        // {
        //     var user = await _likedRepository.GetUserLikes(predicate, User.GetUserId());

        //     return Ok(user);
        // }

         [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes([FromQuery] LikesParams param)
        {
            param.UserId= User.GetUserId();
            var user = await _likedRepository.GetUserLikes(param);
            Response.AddPaginationHeader(user.CurrentPage,user.PageSize,user.TotalCount,user.TotalPages);
           
            return Ok(user);
        }
    }
}