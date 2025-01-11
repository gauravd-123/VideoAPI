using System.ComponentModel.DataAnnotations;

namespace VideoAPI.Models
{
	public class Login
	{
        public int userId { get; set; }
        [Required]
        public string userName { get; set; }
        public string createOn { get; set; } = "";
        [Required]
        public string? password { get; set; }
        public string? contactNo { get; set; } = "";

    }

    public class LoginRs
    {
        public string status { get; set; } = "";
        public string message { get; set; } = "";
    }
}
