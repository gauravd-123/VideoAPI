using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoAPI.BL;
using VideoAPI.DL;
using VideoAPI.Models;

namespace VideoAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
        private readonly IConfiguration _config;
        private readonly LoginBL loginBl;
		private readonly LoginDL loginDL;

		public LoginController(IConfiguration config)
        {
            _config = config;
            //loginBl = bl;
            //loginDL = dl;
        }

        [HttpGet("healthCheck")]
        public string healthCheck()
        {
            return "Working Fine";
        }

        [HttpGet("getUser")]
		public async Task<ActionResult> GetUser(int userId)
        {
            Login login = new Login();
            LoginBL bl = new LoginBL(_config);
            login = await bl.getUser(userId);
            return Ok(login);
        }
    }
}
