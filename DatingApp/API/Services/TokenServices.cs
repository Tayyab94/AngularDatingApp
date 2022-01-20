using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly  SymmetricSecurityKey _key;
        public TokenServices(IConfiguration configuration)
        {
            _key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["tokenKey"]));

        }

        // We are using this function to create JWT token.. 
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
            };

            var creds= new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription= new SecurityTokenDescriptor
            {
                    Subject= new ClaimsIdentity(claims),
                    Expires= DateTime.Now.AddDays(7),
                     SigningCredentials= creds
            };

            var tokenHandler= new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }


    }
}