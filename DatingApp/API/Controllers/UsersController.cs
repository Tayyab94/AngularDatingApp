using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
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

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            // this._context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users=await userRepository.GetUsersAsync();
            var returnUsers = mapper.Map<IEnumerable<MemberDTO>>(users);
            return Ok(returnUsers);
        }


        // [HttpGet("{id}")]
        // [Authorize]
        // public async Task<ActionResult<AppUser>> GetUser(int id)
        // {
        //     var user =await this._context.AppUsers.FindAsync(id);
        //     return user;
        // }

            [HttpGet("{username}")]
        [Authorize]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var user =mapper.Map<MemberDTO>(await this.userRepository.GetUserByUsernameAsync(username));
            return user;
        }
    }
}