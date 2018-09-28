using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AerionDyseti.Api.Shared.Controllers
{
    [Route("api/debug")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        // GET api/debug
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/debug/world
        [HttpGet("{value}")]
        public ActionResult<string> Get(string value)
        {
            return $"Hello, {value}";
        }

        // GET api/debug/secret
        [HttpGet("secret")]
        [Authorize]
        public ActionResult<string> GetSecret()
        {
            return "Checks out.";
        }


    }

}
