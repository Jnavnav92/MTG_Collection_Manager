using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class JWTHelper
    {
        public static async Task<string> GenerateJwtTokenAsync(string sUserName, string sJwtSecret)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, sUserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (Encoding.UTF8.GetBytes(sJwtSecret).Length < 32)
            {
                sJwtSecret = sJwtSecret.PadRight((256 / 8), '\0');
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sJwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: "MtgCollectionMgr",
                                             audience: "MtgCollectionMgr",
                                             claims: claims,
                                             expires: DateTime.Now.AddMinutes(30),
                                             signingCredentials: creds);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
