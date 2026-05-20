using BugTrackingSystem.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BugTrackingSystem.Services
{
    public class JwtService
    {
        public string GenerateToken(User user, IList<string> permissions = null)
        {
            var secretKey     = ConfigurationManager.AppSettings["JwtSecretKey"];
            var issuer        = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience      = ConfigurationManager.AppSettings["JwtAudience"];
            var expireMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["JwtExpireMinutes"]);

            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("UserId",   user.Id.ToString()),
                new Claim("FullName", user.FullName),
                new Claim("Email",    user.Email),
                new Claim("Role",     user.Role.Name)
            };

            if (permissions != null)
            {
                foreach (var perm in permissions)
                    claims.Add(new Claim("Permission", perm));
            }

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims:             claims,
                expires:            DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
