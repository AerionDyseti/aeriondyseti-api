using AerionDyseti.API.Auth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;

namespace AerionDyseti.Tests
{
    public class FakeSignInManager : SignInManager<AerionDysetiUser>
    {
        public FakeSignInManager() : base(
            new Mock<FakeUserManager>().Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<AerionDysetiUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<AerionDysetiUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object
        )
        { }
    }



    public class FakeUserManager : UserManager<AerionDysetiUser>
    {
        public FakeUserManager() : base(
                  new Mock<IUserStore<AerionDysetiUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<AerionDysetiUser>>().Object,
                  new IUserValidator<AerionDysetiUser>[0],
                  new IPasswordValidator<AerionDysetiUser>[0],
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<AerionDysetiUser>>>().Object
        )
        { }

        public override Task<IdentityResult> CreateAsync(AerionDysetiUser user, string password)
        {
            return (password == "failme")
                ? Task.FromResult(
                    IdentityResult.Failed(
                        new IdentityError() { Description = "Identity Error 1" },
                        new IdentityError() { Description = "Identity Error 2" }
                    )
                )
                : Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(AerionDysetiUser user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(AerionDysetiUser user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

    }

}
