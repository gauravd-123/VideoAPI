using System.ComponentModel.DataAnnotations;

namespace VideoAPI.Models
{
	public class Login
	{
        public int userId { get; set; }
        public string userName { get; set; }
        public string createOn { get; set; } = "";

    }
}
