using AerionDyseti.API.Auth.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace AerionDyseti.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Pull options from Configuration.
            string jwtIssuer = Configuration["JwtSettings:Issuer"];
            string jwtAudience = Configuration["JwtSettings:Audience"];
            string jwtSecret = Configuration["JwtSettings:Secret"];


            // Setup EntityFramework.
            services.AddDbContext<AerionDysetiContext>(o =>
            {
                o.UseInMemoryDatabase("DevDb");
            });



            // Identity Services.
            services.AddIdentity<AerionDysetiUser, IdentityRole>(options =>
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



            // Add the JwtService to IoC container.
            services.Configure<JwtOptions>(opt =>
            {
                opt.Issuer = jwtIssuer;
                opt.Audience = jwtAudience;
                opt.Secret = jwtSecret;
            });


            // Only use JwtBearer for authentication.
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Use JwtBearer Authentication with the given TokenValidation Parameters.
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!  
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

                    // Validate the JWT Issuer (iss) claim  
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,

                    // Validate the JWT Audience (aud) claim  
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,

                    // Validate the token expiry  
                    ValidateLifetime = true

                };
            });


            // Add MVC services.
            services.AddMvc()

            // Use 2.1 features. 
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)

            // Set JSON options to indent JSON output and ignore null values when serializing.
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.Formatting = Formatting.Indented;
                opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                opt.SerializerSettings.Converters.Add(new StringEnumConverter());
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
