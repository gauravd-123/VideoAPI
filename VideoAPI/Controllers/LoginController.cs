using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VideoAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
        public LoginController()
        {
            
        }

        [HttpGet]
        public string healthCheck()
        {
            return "Working Fine";
        }
    }
}
