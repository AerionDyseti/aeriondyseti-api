using AerionDyseti.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AerionDyseti.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds a JWT service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void AddJwtService(this IServiceCollection services, string issuer, string audience, string secret)
        {
            // Add the JwtService to IoC container.
            services.Configure<JwtService>(opt =>
            {
                opt.Issuer = issuer;
                opt.Audience = audience;
                opt.Secret = secret;
            });
        }

    }
}
