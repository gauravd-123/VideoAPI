using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using VideoAPI.Models;
using VideoAPI.Services;

namespace VideoAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class R2Controller : ControllerBase
	{
		private readonly R2Service _r2Service;

		private readonly string _accessKey = "611994db09a073f3e1c518417041c288";
		private readonly string _secretKey = "5b8ad888db67f43b31d82a040e2be22862ac46d76afb279c3c55abd2390909a3";
		private readonly string _bucket = "videoapp";
		private readonly string _accountId = "52ba3e303e01c02fc3b413f58610f217";

		//"AccessKey": "611994db09a073f3e1c518417041c288",
  //      "SecretKey": "5b8ad888db67f43b31d82a040e2be22862ac46d76afb279c3c55abd2390909a3",
  //      "BucketName": "videoapp",
  //      "ServiceURL": "https://52ba3e303e01c02fc3b413f58610f217.r2.cloudflarestorage.com",
  //      "AccountId": "52ba3e303e01c02fc3b413f58610f217",
		public R2Controller(R2Service r2Service)
		{
			_r2Service = r2Service;
		}

		[HttpPost("upload")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> Upload([FromForm] IFormFile file)
		{
			if (file == null || file.Length == 0)
				return BadRequest("No file uploaded.");

			//await _r2Service.ListBuckets();
			//await _r2Service.ListObjectsV2();
			//await _r2Service.PutObject();
			//await _r2Service.GetObject();


			//string url = await _r2Service.GeneratePreSignedUrlAsync(file.FileName);
			var fileUrl = await _r2Service.UploadFileAsync(file);
			return Ok(new { url = fileUrl });
		}

		[HttpPost("GetVideoUrl")]
		public async Task<R2Urls> GetVideoUrl()
		{
			R2Urls obj = await _r2Service.GetUrl();
			return obj;
		}
		
		[HttpGet("GetVideoUrlByKey")]
		public async Task<R2Urls> GetVideoUrlByKey([FromQuery]string key)
		{
			R2Urls obj = await _r2Service.GetVideoDataByName(key);
			return obj;
		}

		[HttpGet("GetAvailble")]
		public async Task<R2Urls> GetAvailbleVideos()
		{
			R2Urls obj = await _r2Service.GetAvailVideos();
			return obj;
		}
		/////////////////////////////////////////////////////////////////////////////////////////

		[HttpPost("initiate")]
		public async Task<IActionResult> InitiateMultipartUpload(string objectName)
		{
			string url = $"https://{_accountId}.r2.cloudflarestorage.com/{_bucket}/{objectName}?uploads";

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = GetAuthorizationHeader("POST", url, _bucket, objectName, "auto");
			var response = await client.PostAsync(url, null);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				return Ok(content); // Returns the uploadId in the response
			}
			return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
		}

		[HttpPut("upload-part")]
		public async Task<IActionResult> UploadPart(string uploadId, string objectName, int partNumber, [FromBody] byte[] fileChunk)
		{
			byte[] chunkData = fileChunk; // Read the chunk data
			string payloadHash = ToHexString(SHA256Hash(chunkData.ToString()));
			if (string.IsNullOrEmpty(payloadHash))
			{
				payloadHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
			}
			string url = $"https://{_accountId}.r2.cloudflarestorage.com/{_bucket}/{objectName}?uploadId={uploadId}&partNumber={partNumber}";

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = GetAuthorizationHeader("PUT", url, _bucket, objectName, "auto", payloadHash);
			var content = new ByteArrayContent(fileChunk);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

			var response = await client.PutAsync(url, content);
			if (response.IsSuccessStatusCode)
			{
				return Ok(response.Headers.GetValues("ETag").FirstOrDefault());
			}
			return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
		}

		[HttpPost("complete")]
		public async Task<IActionResult> CompleteMultipartUpload(string uploadId, string objectName, [FromBody] string xmlBody)
		{
			string url = $"https://{_accountId}.r2.cloudflarestorage.com/{_bucket}/{objectName}?uploadId={uploadId}";

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = GetAuthorizationHeader("POST", url, _bucket, objectName, "auto");
			var content = new StringContent(xmlBody);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

			var response = await client.PostAsync(url, content);
			if (response.IsSuccessStatusCode)
			{
				return Ok(await response.Content.ReadAsStringAsync());
			}
			return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
		}

		private AuthenticationHeaderValue GetAuthorizationHeader(string method, string url, string bucket, string objectName, string region, string payloadHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")
		{
			//if (string.IsNullOrEmpty(payloadHash))
			//{
			//	payloadHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
			//}

			//string payloadHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
			
			const string service = "s3";
			//string accessKey = "YOUR_ACCESS_KEY"; // Replace with your Cloudflare Access Key
			//string secretKey = "YOUR_SECRET_KEY"; // Replace with your Cloudflare Secret Key
			string date = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
			string shortDate = DateTime.UtcNow.ToString("yyyyMMdd");

			// Parse host from URL
			var uri = new Uri(url);
			string host = uri.Host;

			// Create Canonical Request
			string canonicalUri = $"/{bucket}/{objectName}";
			string canonicalQueryString = uri.Query.TrimStart('?');
			string canonicalHeaders = $"host:{host}\n";
			string signedHeaders = "host";
			string canonicalRequest = $"{method}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";

			// Create String to Sign
			string credentialScope = $"{shortDate}/{region}/{service}/aws4_request";
			string stringToSign = $"AWS4-HMAC-SHA256\n{date}\n{credentialScope}\n{ToHexString(SHA256Hash(canonicalRequest))}";

			// Create Signature
			byte[] signingKey = GetSignatureKey(_secretKey, shortDate, region, service);
			string signature = ToHexString(HMACSHA256(stringToSign, signingKey));

			// Create Authorization Header
			string authorization = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

			return new AuthenticationHeaderValue("Authorization", authorization);
		}

		// Helper Methods
		private static byte[] SHA256Hash(string data)
		{
			using var sha256 = SHA256.Create();
			return sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
		}

		private static byte[] HMACSHA256(string data, byte[] key)
		{
			using var hmac = new HMACSHA256(key);
			return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
		}

		private static byte[] GetSignatureKey(string key, string date, string region, string service)
		{
			byte[] kDate = HMACSHA256(date, Encoding.UTF8.GetBytes("AWS4" + key));
			byte[] kRegion = HMACSHA256(region, kDate);
			byte[] kService = HMACSHA256(service, kRegion);
			return HMACSHA256("aws4_request", kService);
		}

		private static string ToHexString(byte[] data)
		{
			return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
		}

	}
}
 