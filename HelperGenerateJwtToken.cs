using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace IdentityPatika;

public class HelperGenerateJwtToken
{
    public static string GenerateToken(string email, string key, string issuer, string audience)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
        };

        var token = new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims,
            expires: DateTime.Now.AddMinutes(30));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// issuer bunlari appsettings.json kismindan aliyor
// token uretme sinifi
// bunu Controller icindeki login endpointi icinde kullaniyoruz -> oradan parametre verileri alacak