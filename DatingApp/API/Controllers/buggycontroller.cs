using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class buggycontroller : ControllerBase
    {
        private readonly DataContext context;

        public buggycontroller(DataContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret Text";
        }

        [HttpGet("not-found")]

        public ActionResult<string> GetNotFound()
        {
            var thing= context.AppUsers.Find(-1);
            if(thing== null) return NotFound();

            return Ok(thing);
        }

        
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing= context.AppUsers.Find(-1);
            var thingToReturn= thing.ToString();
            return thingToReturn;
        }

         [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not good Request");
        }
    }
}