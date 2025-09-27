using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shop.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Shop.Services
{

    /// <summary>
    ///  O tokenHandler é responsavel por gerar e manipular o nosso token
    ///  Necessitamos da nossa chave que secreta, porque sempre que necessitramos trabalhar como tokens nos invocarmos
    /// 
    ///  tokenDescriptor - descrição do que terá dentro do token
    ///   new ClaimsIdentity-( new Claim[]) permite-nos trabalhar com user.Identity.Name e Role e essa informação nos temos disponiveis dentro da nossa aplicação  
    ///  
    ///  
    /// 
    /// </summary>
    public static class TokenService
    {
        public static string GenerateToken(User user)
        {
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}