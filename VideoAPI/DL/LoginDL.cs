using Npgsql;
using System.Data;
using VideoAPI.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace VideoAPI.DL
{
    public class LoginDL
    {
        private readonly IConfiguration _config;
        //public string connString = "postgresql://postgres.dnkhrciegrznuftvfphp:Electronics777&@aws-0-ap-south-1.pooler.supabase.com:6543/postgres";
		public string connString = "Host=aws-0-ap-south-1.pooler.supabase.com;Port=6543;Username=postgres.dnkhrciegrznuftvfphp;Password=Electronics777&;Database=postgres;SslMode=Require;TrustServerCertificate=True;";


		public LoginDL(IConfiguration config)
        {
            _config = config;
        }

        public async Task<Login> getUserFromDb(int userid)
        {
            Login login = new Login();
            string connectionString = _config.GetConnectionString("connectionString");
            try
            {
				using (IDbConnection conn = new NpgsqlConnection(connString))
				{
					conn.Open();
					string query = "SELECT * FROM \"Login_User\" WHERE userid = @userid";
					login = await conn.QueryFirstOrDefaultAsync<Login>(query, new { userid });
				}
			} 
            catch (Exception ex)
            {

            }
            
            return login;
        }
    }
}
