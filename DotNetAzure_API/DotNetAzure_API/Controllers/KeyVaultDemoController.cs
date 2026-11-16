using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAzure_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyVaultDemoController : ControllerBase
    {
        private readonly IConfiguration _configurataion;

        public KeyVaultDemoController(IConfiguration configuration)
        {
            _configurataion = configuration;
        }

        [HttpGet]
        public IActionResult GetSecret()
        {
            var dbConnectionString = _configurataion["DbConnectionString"];
            return Ok(new { dbConnectionString = dbConnectionString });
        }
    }
}
