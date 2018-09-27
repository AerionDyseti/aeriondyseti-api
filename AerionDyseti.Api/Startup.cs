using AerionDyseti.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            string jwtIssuer = Configuration["JwtSettings:Issuer"];
            string jwtAudience = Configuration["JwtSettings:Audience"];
            string jwtSecret = Configuration["JwtSettings:Secret"];

            services.SetupEntityFramework();
            services.SetupIdentityService();
            services.SetupJwtService(jwtIssuer, jwtAudience, jwtSecret);
            services.SetupAuthentication(jwtIssuer, jwtAudience, jwtSecret);
            services.SetupMVC();

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
