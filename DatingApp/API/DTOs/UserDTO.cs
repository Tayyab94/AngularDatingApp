using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UserDTO
    {
        public string userName { get; set; }

        public string Token { get; set; }

        public string photoUrl{get;set;}

        public string knownAs {get;set;}
    }
}