using VideoAPI.DL;
using VideoAPI.Models;

namespace VideoAPI.BL
{
	public class LoginBL
	{
        //private static IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        protected readonly IConfiguration _config;
        private readonly LoginDL loginDL;

        public LoginBL(IConfiguration config)
        {
            _config = config;
            //loginDL = dl;
        }

        public async Task<Login> getUser(int userId)
        {
            Login login = new Login();
            LoginDL dl = new LoginDL(_config);
            login = await dl.getUserFromDb(userId);
            return login;
        }

    }
}
