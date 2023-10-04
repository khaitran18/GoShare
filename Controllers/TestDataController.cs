using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDataController : Controller
    {
        private readonly postgresContext context;

        public TestDataController(postgresContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (await Task.FromResult(context.Users.Any(u => u.Phone.Equals("0919651361"))))
            {
                return Ok();
            }
            
            else return BadRequest();
        }
    }
}
