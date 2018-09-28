using AerionDyseti.API.Auth.Models;
using AerionDyseti.API.Auth.Models.RequestDTOs;
using AerionDyseti.API.Auth.Models.ResponseDTOs;
using AerionDyseti.API.Shared.Models.ResponseDTOs;
using AerionDyseti.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AerionDyseti.API.Auth.Controllers
{
    /// <summary>
    /// Controller to handle authentication and user management, such as registration and fetching an access token.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly JwtOptions jwtOptions;
        private readonly SignInManager<AerionDysetiUser> signInManager;
        private readonly UserManager<AerionDysetiUser> userManager;

        /// <summary>
        /// Creates an instance of the AuthController object.
        /// </summary>
        /// <param name="jwtOptions">The JWT service to be injected into this controller.</param>
        /// <param name="userManager">The Identity User Manager to be injected into this controller.</param>
        /// <param name="signInManager">The Identity Sign-In Manager to be injected into this controller.</param>
        public AuthController(IOptions<JwtOptions> jwtOptions, UserManager<AerionDysetiUser> userManager, SignInManager<AerionDysetiUser> signInManager)
        {
            this.jwtOptions = jwtOptions.Value;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// Logs a user in, generating a token for use by that user.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
        {
            AerionDysetiUser user = await userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return BadRequest(new ErrorResponse { Errors = new List<string> { "The provided email and password does not match any accounts." } });
            }
            if (user.ApprovalDate == null)
            {
                return BadRequest(new ErrorResponse { Errors = new List<string> { "The provided email is not yet approved to use this system." } });
            }

            SignInResult result = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponse { Errors = new List<string> { "The provided email and password did not match any accounts." } });
            }

            await signInManager.SignInAsync(user, isPersistent: false);

            IList<Claim> userClaims = await userManager.GetClaimsAsync(user);

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
            SigningCredentials signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Iss, jwtOptions.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, jwtOptions.Audience),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToUnixTimestamp().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUnixTimestamp().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddHours(5).ToUnixTimestamp().ToString())
            };

            claims.AddRange(userClaims);

            JwtSecurityToken token = new JwtSecurityToken(claims: claims, signingCredentials: signingCreds);

            TokenResponse tokenResponse = new TokenResponse { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = token.ValidTo.ToUnixTimestamp() };
            return Ok(tokenResponse);

        }

        /// <summary>
        /// Registers a new user, adding them to the Database.
        /// </summary>
        /// <param name="registerRequest">A request object containing the information needed to create a new user.</param>
        /// <returns>An ActionResult corresponding to the HTTP Status Code for the given transaction.</returns>
        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {

            AerionDysetiUser user = new AerionDysetiUser
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                CreationDate = DateTime.UtcNow,
                ApprovalDate = null
            };

            IdentityResult result = await userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponse { Errors = result.Errors.Select(err => err.Description).ToList() });
            }

            // Create the JWT Token and return
            return Ok(new SuccessResponse());

        }


        /// <summary>
        /// Approve a user to use the API.
        /// </summary>
        /// <param name="email">Email of the user to approve for use in the API.</param>
        /// <returns>An ActionResult corresponding to the HTTP Status Code for the given transaction.</returns>
        [HttpPost]
        [Route("approve")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> ApproveUser([FromBody][Required] string Email)
        {

            AerionDysetiUser user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return BadRequest(new ErrorResponse { Errors = new List<string> { "The provided email does not match any accounts." } });
            }

            user.ApprovalDate = DateTime.UtcNow;

            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponse { Errors = result.Errors.Select(e => e.Description).ToList() });
            }

            return Ok(new SuccessResponse());
        }


    }

}
