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

        [HttpPost("userLogin")]
        public async Task<LoginRs> ValidateUser(Login loginRq)
        {
            LoginRs rs = new LoginRs();
			LoginBL bl = new LoginBL(_config);
            rs = await bl.userValidate(loginRq);
			return rs;
        }

		[HttpPost("createUser")]
		public async Task<DBRS> CreateUser(Login loginRq)
		{
            DBRS Dbrs = new DBRS();
			LoginBL bl = new LoginBL(_config);
			Dbrs = await bl.createUser(loginRq);
			return Dbrs;
		}
	}
}
