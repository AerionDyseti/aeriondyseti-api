using AerionDyseti.API.Shared.Models;
using AerionDyseti.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;

namespace AerionDyseti.Extensions
{
    public static class ServiceExtensions
    {

        /// <summary>
        /// Adds a JWT service
        /// </summary>
        public static void SetupJwtService(this IServiceCollection services, string issuer, string audience, string secret)
        {
            // Add the JwtService to IoC container.
            services.Configure<JwtService>(opt =>
            {
                opt.Issuer = issuer;
                opt.Audience = audience;
                opt.Secret = secret;
            });
        }


        /// <summary>
        /// Adds the Identity Core services, sets password and user requirements, and adds EF as the Identity Store.
        /// </summary>
        public static void SetupIdentityService(this IServiceCollection services)
        {
            services.AddIdentityCore<AerionDysetiUser>(options =>
            {
                options.Password = new PasswordOptions
                {
                    RequireDigit = true,
                    RequiredLength = 6,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false
                };

                options.User = new UserOptions
                {
                    RequireUniqueEmail = true
                };
            })
            .AddEntityFrameworkStores<AerionDysetiContext>();
        }


        /// <summary>
        /// Setup JWT Authentication using a Bearer token scheme.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="secret"></param>
        public static void SetupAuthentication(this IServiceCollection services, string issuer, string audience, string secret)
        {
            // Only use JwtBearer for authentication.
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Use JwtBearer Authentication with the given TokenValidation Parameters.
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!  
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),

                // Validate the JWT Issuer (iss) claim  
                ValidateIssuer = true,
                ValidIssuer = issuer,

                // Validate the JWT Audience (aud) claim  
                ValidateAudience = true,
                ValidAudience = audience,

                // Validate the token expiry  
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            });
        }


        /// <summary>
        /// Setup EntityFramework.
        /// TODO: Use MySQL instead of In-Memory database.
        /// </summary>
        public static void SetupEntityFramework(this IServiceCollection services)
        {
            services.AddDbContext<AerionDysetiContext>(o =>
            {
                o.UseInMemoryDatabase("DevDb");
            });
        }


        /// <summary>
        /// Setup the MVC services with 2.1 compatibility and JSON serialization options.
        /// </summary>
        public static void SetupMVC(this IServiceCollection services)
        {
            // Add MVC services.
            services.AddMvc()

            // Use 2.1 features. 
            // This will be unneccesary when ASP.NET Core 3.0 comes out.
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)

            // Set JSON options to indent JSON output and ignore null values when serializing.
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.Formatting = Formatting.Indented;
                opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

        }

    }

}
