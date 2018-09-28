using AerionDyseti.API.Auth.Controllers;
using AerionDyseti.API.Auth.Models;
using AerionDyseti.API.Auth.Models.RequestDTOs;
using AerionDyseti.API.Shared.Models.ResponseDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AerionDyseti.Tests
{

    [TestClass]
    public class AuthControllerTests
    {
        // Common Mocks.
        private readonly IOptions<JwtOptions> opts = Options.Create(new JwtOptions() { Audience = "test", Issuer = "test", Secret = "test" });
        private readonly SignInManager<AerionDysetiUser> fakeSignInManager = new FakeSignInManager();
        private readonly UserManager<AerionDysetiUser> fakeUserManager = new FakeUserManager();


        [TestMethod]
        public async Task Register_ReturnsSuccessResult_WhenGivenValidRequest()
        {
            AuthController controllerUnderTest = new AuthController(opts, fakeUserManager, fakeSignInManager);
            RegisterRequest validRequest = new RegisterRequest
            {
                Email = "test@aeriondyseti.com",
                FirstName = "Kevin",
                LastName = "Whiteside",
                Password = "testpass123"
            };

            OkObjectResult result = (await controllerUnderTest.Register(validRequest)) as OkObjectResult;
            Assert.IsNotNull(result);

            SuccessResponse val = result.Value as SuccessResponse;
            Assert.IsNotNull(val);
            Assert.IsTrue(val.Success);
        }

        [TestMethod]
        public async Task Register_ReturnsErrorResponse_WhenGivenInvalidRequest()
        {
            AuthController controllerUnderTest = new AuthController(opts, fakeUserManager, fakeSignInManager);
            RegisterRequest validRequest = new RegisterRequest
            {
                Email = "test@aeriondyseti.com",
                FirstName = "Kevin",
                LastName = "Whiteside",
                Password = "failme"
            };

            BadRequestObjectResult result = (await controllerUnderTest.Register(validRequest)) as BadRequestObjectResult;
            Assert.IsNotNull(result);

            ErrorResponse val = result.Value as ErrorResponse;
            Assert.IsNotNull(val);
            Assert.IsTrue(val.Errors.Contains("Identity Error 1"));
            Assert.IsTrue(val.Errors.Contains("Identity Error 2"));
        }

    }

}
