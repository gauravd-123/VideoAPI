namespace VideoAPI.Models
{
	public class R2
	{

	}

	public class R2Urls
	{
		public string? status { get; set; } = "";
        public List<R2Data> urlList { get; set; }
    }

	public class R2Data
	{
        public string? FileName { get; set; }
		public string? Url { get; set; } = "";

		public string? poster { get; set; } = "";
    }
}
