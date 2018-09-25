using AerionDyseti.API.Shared.Models;
using AerionDyseti.Extensions;
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
using Newtonsoft.Json.Serialization;
using System;
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

            services.AddJwtService(
                Configuration["JwtSettings:Issuer"],
                Configuration["JwtSettings:Audience"],
                Configuration["JwtSettings:Secret"]
            );


            // Add EF Context Service.
            // TODO: Switch this to MySQL instead of in-memory.
            string dbStr = Configuration["dbConnectionString"];
            services.AddDbContext<AerionDysetiContext>(
                o => o.UseInMemoryDatabase("DevDb")
            );


            // Use Microsoft Identity for user management.
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
            // Use EF to store Identity information.
            .AddEntityFrameworkStores<AerionDysetiContext>();


            // Add Authentication Middleware
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                // Use JwtBearer Authentication with the given TokenValidation Parameters.
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!  
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Secret"])),

                    // Validate the JWT Issuer (iss) claim  
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["JwtSettings:Issuer"],

                    // Validate the JWT Audience (aud) claim  
                    ValidateAudience = true,
                    ValidAudience = Configuration["JwtSettings:Audience"],

                    // Validate the token expiry  
                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero
                });

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
                    }
                );


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

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
