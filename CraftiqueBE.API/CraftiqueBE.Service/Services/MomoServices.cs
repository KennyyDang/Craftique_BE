using CraftiqueBE.Data.Models.MomoModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class MomoServices : IMomoService
	{
		private readonly IConfiguration _configuration;
		private readonly HttpClient _httpClient;

		public MomoServices(IConfiguration configuration)
		{
			_configuration = configuration;
			_httpClient = new HttpClient();
		}

		public async Task<MomoPaymentResponseModel> CreatePaymentAsync(MomoPaymentRequestModel model)
		{
			var endpoint = _configuration["MoMo:Endpoint"];
			var partnerCode = _configuration["MoMo:PartnerCode"];
			var accessKey = _configuration["MoMo:AccessKey"];
			var secretKey = _configuration["MoMo:SecretKey"];

			string requestId = Guid.NewGuid().ToString();
			string orderId = Guid.NewGuid().ToString();
			string redirectUrl = model.RedirectUrl ?? "https://momo.vn";
			string ipnUrl = model.IpnUrl ?? "https://webhook.site/your-temporary-callback-url";
			string orderInfo = model.Description ?? "Nap tien vao vi Craftique";
			string amount = model.Amount.ToString();
			string extraData = "";

			// Create raw signature
			string rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType=captureWallet";
			string signature = GenerateSignature(rawHash, secretKey);

			var requestData = new
			{
				partnerCode,
				accessKey,
				requestId,
				amount,
				orderId,
				orderInfo,
				redirectUrl,
				ipnUrl,
				extraData,
				signature,
				requestType = "captureWallet",
				lang = "vi"
			};

			var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(endpoint, content);
			var json = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<MomoPaymentResponseModel>(json);

			return result;
		}

		private string GenerateSignature(string rawData, string key)
		{
			var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
			var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
		}
	}
}
