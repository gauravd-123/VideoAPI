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

		public async Task<LoginRs> userValidate(Login loginRq)
		{
			LoginRs login = new LoginRs();
			LoginDL dl = new LoginDL(_config);
			login = await dl.validate(loginRq);
			return login;
		}

        public async Task<DBRS> createUser(Login rq)
        {
            LoginDL dl = new LoginDL(_config);
            DBRS Dbrs = new DBRS();
            //Dbrs = await dl.createUser(rq);
            //int userId = await dl.createUserId();
            bool userExists = await dl.checkUserAlreadyExists(rq.userName, rq.password);
            if (!userExists)
            {
				Dbrs = await dl.createUser(rq);
			} else
            {
                Dbrs.status = "Failure";
                Dbrs.message = "User already exists";
            }
			return Dbrs;
        }

	}
}
