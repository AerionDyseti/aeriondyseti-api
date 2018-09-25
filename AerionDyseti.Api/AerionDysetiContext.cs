using AerionDyseti.API.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AerionDyseti
{
    public class AerionDysetiContext : IdentityDbContext<AerionDysetiUser>
    {
        public AerionDysetiContext(DbContextOptions<AerionDysetiContext> options) : base(options)
        {
        }
    }
}