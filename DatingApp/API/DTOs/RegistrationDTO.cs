using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegistrationDTO
    {
        [Required(ErrorMessage ="User Name is Requried")]
        public string username { get; set; }

        [Required(ErrorMessage ="Password is Requried")]
        [StringLength(8,MinimumLength =4)]
        public string password { get; set; }

        [Required] public  string KnownAs {get;set;}
                [Required] public  DateTime DateOfBirth {get;set;}
                        [Required] public  string Gender {get;set;}
                                [Required] public  string City {get;set;}
                                        [Required] public  string Country {get;set;}
    }
}