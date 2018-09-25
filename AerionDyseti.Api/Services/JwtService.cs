using AerionDyseti.API.Shared.Models;
using AerionDyseti.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AerionDyseti.Services
{
    // Service to handle generating a token response from a user object and a list of Claims.
    public class JwtService
    {
        public string Audience;
        public string Issuer;
        public string Secret;


        public TokenResponse GenerateTokenResponse(AerionDysetiUser user, IList<Claim> userClaims)
        {
            SigningCredentials signingCreds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                SecurityAlgorithms.HmacSha256
            );

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Iss, Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, Audience),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToUnixTimestamp().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUnixTimestamp().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddHours(5).ToUnixTimestamp().ToString())
            };

            claims.AddRange(userClaims);

            JwtSecurityToken token = new JwtSecurityToken(claims: claims, signingCredentials: signingCreds);

            return new TokenResponse { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = token.ValidTo.ToUnixTimestamp() };
        }
    }
}
