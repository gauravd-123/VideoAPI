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
		public string connString = "Host=aws-0-ap-south-1.pooler.supabase.com;Port=6543;Username=postgres.dnkhrciegrznuftvfphp;Password=Electronics777&;Database=postgres;SslMode=Require;TrustServerCertificate=True;Timeout=300;Pooling=false;MinPoolSize=1;MaxPoolSize=10;CommandTimeout=300;KeepAlive = 300;";


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

		public async Task<LoginRs> validate(Login loginRq)
		{
			LoginRs loginRs = new LoginRs();
			string connectionString = _config.GetConnectionString("connectionString");
			try
			{
				using (IDbConnection conn = new NpgsqlConnection(connString))
				{
					conn.Open();
					string query = "SELECT COUNT(*) FROM \"Login_User\" WHERE username = @username AND password = @password";
					int i = await conn.ExecuteScalarAsync<int>(query, loginRq);
					if(i > 0)
					{
						loginRs.status = "Success";
						loginRs.message = "Valid user";
					} else
					{
						loginRs.status = "Failure";
						loginRs.message = "Invalid user";
					}
				}
			}
			catch (Exception ex)
			{
				loginRs.status = "Failure";
				loginRs.message = ex.Message;
			}

			return loginRs;
		}

		public async Task<DBRS> createUser(Login rq)
		{
			DBRS dBrs = new DBRS();
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(connString))
				{
					await conn.OpenAsync();
					string query = "INSERT INTO \"Login_User\" (userid, username, password, created_on, is_active, contactNo, emailId) VALUES (nextval('\"DUAL\"'), @username, @password, @created_on, @is_active, @contactNo, @emailId)";
					var param = new
					{
						//userid = userid,
						username = rq.userName,
						password = rq.password,
						created_on = DateTime.Now,
						is_active = 1,
						contactNo = rq.contactNo
					};
					int i = await conn.ExecuteAsync(query, param);
					if (i > 0)
					{
						dBrs.status = "Success";
						dBrs.message = "User Added successfully";
					}
					else
					{
						dBrs.status = "Failure";
						dBrs.message = "User not added in DB";
					}
				}
			} catch (Exception ex)
			{
				dBrs.status = "Failure";
				dBrs.message = ex.Message +"StackTrace: "+ ex.StackTrace;
			}
				
			return dBrs;
		}

		private async Task<int> createUserId()
		{
			int userID = 0;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(connString))
				{
					string query = "SELECT nextval('\"DUAL\"')";
					userID = await conn.ExecuteScalarAsync<int>(query);
				}
			} catch(Exception ex)
			{

			}
			
			return userID;
		}

		public async Task<bool> checkUserAlreadyExists(string username, string password)
		{
			DBRS dBrs = new DBRS();
			bool flag = false;
			try
			{
				using (NpgsqlConnection conn = new NpgsqlConnection(connString))
				{
					await conn.OpenAsync();
					string query = "SELECT * FROM \"Login_User\" WHERE username=@username AND password=@password";
					var param = new
					{
						username = username,
						password = password,
					};
					int i = await conn.ExecuteScalarAsync<int>(query, param);
					if (i > 0)
					{
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				dBrs.status = "Failure";
				dBrs.message = ex.Message + "StackTrace: " + ex.StackTrace;
			}

			return flag;
		}
	}
}
