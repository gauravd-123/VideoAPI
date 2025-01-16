using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System.Diagnostics.Eventing.Reader;
using VideoAPI.DL;
using VideoAPI.Models;

namespace VideoAPI.Services
{
	public class R2Service
	{
		private readonly AmazonS3Client _s3Client;
		private readonly string _bucketName;
		private readonly string _accessKey;
		private readonly string _secretKey;
		private readonly string _serviceUrl;
		private readonly string _accountId;

		private static IAmazonS3 s3Client;



		public R2Service(IConfiguration configfiguration)
        {
             _accessKey = configfiguration["R2:AccessKey"];
             _secretKey = configfiguration["R2:SecretKey"];
             _serviceUrl = configfiguration["R2:ServiceURL"];


            _bucketName = configfiguration["R2:BucketName"];
			_accountId = configfiguration["R2:AccountId"];

            var config = new AmazonS3Config
            {
                ServiceURL = _serviceUrl,
				ForcePathStyle = true,

			};

            _s3Client = new AmazonS3Client(_accessKey, _secretKey, config);

			var accessKey = "611994db09a073f3e1c518417041c288";
			var secretKey = "5b8ad888db67f43b31d82a040e2be22862ac46d76afb279c3c55abd2390909a3";
			var credentials = new BasicAWSCredentials(accessKey, secretKey);
			s3Client = new AmazonS3Client(credentials, new AmazonS3Config
			{
				ServiceURL = "https://52ba3e303e01c02fc3b413f58610f217.r2.cloudflarestorage.com",
			});
		}

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = file.FileName,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType,
				AutoCloseStream = true,
				DisablePayloadSigning = true
			};

            var res = await _s3Client.PutObjectAsync(request);
            return $"https://{_bucketName}.r2.cloudflarestorage.com/{file.FileName}";
        }

		public async Task<string> GeneratePreSignedUrlAsync(string fileName)
		{
			TimeSpan time = TimeSpan.FromHours(3);
			//time.Add(60);
			
			var s3Client = new AmazonS3Client(_accessKey, _secretKey, new AmazonS3Config
			{
				ServiceURL = _serviceUrl,
				SignatureVersion = "v4"
			});

			var request = new GetPreSignedUrlRequest
			{
				BucketName = _bucketName,
				Key = fileName,
				Verb = HttpVerb.PUT,
				Expires = DateTime.UtcNow.Add(time),
			};

			return s3Client.GetPreSignedURL(request);
		}


			//"AccessKey": "ef6612fbb5936971d481111c6689b0f4",
			//"SecretKey": "6965ce3842a2b92ea14f43b85aba1377a47b25259b96e89dd825caf132b064eb",
			//"BucketName": "videoapp",
			//"ServiceURL": "https://52ba3e303e01c02fc3b413f58610f217.r2.cloudflarestorage.com"

		

		public async Task ListBuckets()
		{
			var response = await s3Client.ListBucketsAsync();

			foreach (var s3Bucket in response.Buckets)
			{
				Console.WriteLine("{0}", s3Bucket.BucketName);
			}
		}
		// sdk-example
		// my-bucket-name

		public async Task ListObjectsV2()
		{
			var request = new ListObjectsV2Request
			{
				BucketName = "videoapp"
			};

			var response = await s3Client.ListObjectsV2Async(request);

			foreach (var s3Object in response.S3Objects)
			{
				Console.WriteLine("{0}", s3Object.Key);
			}
		}
		// dog.png
		// cat.png

		public async Task PutObject()
		{
			var request = new PutObjectRequest
			{
				FilePath = @"/file.txt",
				BucketName = "videoapp",
				DisablePayloadSigning = true,
			};

			var response = await s3Client.PutObjectAsync(request);

			Console.WriteLine("ETag: {0}", response.ETag);
		}
		// ETag: "186a71ee365d9686c3b98b6976e1f196"

		public async Task GetObject()
		{
			var bucket = "videoapp";
			var key = "file.txt";

			var response = await s3Client.GetObjectAsync(bucket, key);

			Console.WriteLine("ETag: {0}", response.ETag);
		}
		// ETag: "186a71ee365d9686c3b98b6976e1f196"


		public async Task<R2Urls> GetUrl(string searchTerm = "")
		{
			R2Urls urlObj = new R2Urls();
			string baseUrl = "https://pub-cff73a2f5fe14cc5971d15a5bfb13c30.r2.dev/";
			var req = new ListObjectsV2Request
			{
				BucketName = _bucketName
			};
			var res = await s3Client.ListObjectsV2Async(req);

			if (string.IsNullOrEmpty(searchTerm))
			{
				urlObj.urlList = res.S3Objects.Select(obj => new R2Data
				{
					FileName = obj.Key,
					Url = $"{baseUrl}{obj.Key}"
				}).ToList();
			} else
			{
				urlObj.urlList = res.S3Objects
				.Where(obj => obj.Key.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
				.Select(obj => new R2Data
				{
					FileName = obj.Key,
					Url = $"{baseUrl}{obj.Key}"
				}).ToList();
			}

			return urlObj;
		}

		public async Task<R2Urls> GetVideoDataByName(string searchTerm)
		{
			R2Urls list = new R2Urls();
			R2DL dl = new R2DL();
			string word = nameToSearch(searchTerm);
			list = await dl.getVideoData(word);
			return list;

		}

		private static string nameToSearch(string text)
		{
			//string txt = text.Replace("'", "");
			string[] str = text.Split(' ');
			string maxLen = "";
			foreach(string word in str)
			{
				//word.Replace("'", "");
				if(word.Length > maxLen.Length)
				{
					maxLen = word;
				}
			}
			return maxLen;
		}

		public async Task<R2Urls> GetAvailVideos()
		{
			R2Urls list = new R2Urls();
			R2DL dl = new R2DL();
			//string word = nameToSearch(searchTerm);
			list = await dl.availableVideoFromDB();
			return list;

		}

	}
}
