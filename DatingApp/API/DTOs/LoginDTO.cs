using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LoginDTO
    {
         [Required(ErrorMessage ="User Name is Requried")]
        public string username { get; set; }

        [Required(ErrorMessage ="Password is Requried")]
        public string password { get; set; }
    }
}