using Dapper;
using Npgsql;
using System.Data;
using VideoAPI.Models;

namespace VideoAPI.DL
{
	public class R2DL
	{
        private readonly IConfiguration _config;
		public string connString = "Host=aws-0-ap-south-1.pooler.supabase.com;Port=6543;Username=postgres.dnkhrciegrznuftvfphp;Password=Electronics777&;Database=postgres;SslMode=Require;TrustServerCertificate=True;Timeout=300;Pooling=false;MinPoolSize=1;MaxPoolSize=10;CommandTimeout=300;KeepAlive = 300;";

		public R2DL()
        {
            
        }

        public async Task<R2Urls> getVideoData(string searchTerm)
        {
            R2Urls urls = new R2Urls();
            try
            {
				using (IDbConnection conn = new NpgsqlConnection(connString))
				{
					string query = "SELECT FileName FROM \"videometadata_master\" WHERE FileName ILIKE @searchTerm";
					conn.Open();
					var param = new
					{
						searchTerm = $"%{searchTerm}%",
					};
					var res = await conn.QueryAsync<R2Data>(query, param);
					urls.urlList = res.ToList();
					urls.status = "Success";
				}
			}
			catch(Exception ex)
			{
				
			}
            
			return urls;

		}
		public async Task<R2Urls> availableVideoFromDB()
        {
            R2Urls urls = new R2Urls();
            try
            {
				int limit = 10;
				using (IDbConnection conn = new NpgsqlConnection(connString))
				{
					string query = "SELECT FileName, poster FROM \"videometadata_master\" limit :limit";
					conn.Open();
					var param = new
					{
						limit = limit,
					};
					var res = await conn.QueryAsync<R2Data>(query, param);
					urls.urlList = res.ToList();
					urls.status = "Success";
				}
			}
			catch(Exception ex)
			{
				
			}
            
			return urls;

		}
	}
}
