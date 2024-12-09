using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Marketplace.Security
{
    public class JwtTokenProvider
    {
        public static string CreateToken(string secureKey, int expiration, string subject = null)
        {
            // Get secret key bytes
            var tokenKey = Encoding.UTF8.GetBytes(secureKey);

            // Create a token descriptor (represents a token, kind of a "template" for token)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            if (!string.IsNullOrEmpty(subject))
            {
                tokenDescriptor.Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                new System.Security.Claims.Claim(ClaimTypes.Name, subject),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, subject),
                });
            }

            var role = "Admin"; // for example...
            tokenDescriptor.Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim(ClaimTypes.Name, subject),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, subject),
                new System.Security.Claims.Claim(ClaimTypes.Role, role),
            });

            // Create token using that descriptor, serialize it and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return serializedToken;
        }
    }
}
