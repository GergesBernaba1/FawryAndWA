using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayWithFawry.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PayWithFawry.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FawryPaymentController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FawryPaymentController> _logger;
        private readonly IConfiguration _configuration;


        public FawryPaymentController(IHttpClientFactory httpClientFactory, ILogger<FawryPaymentController> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("MakePaymentCharge")]
        public async Task<IActionResult> MakePaymentCharge([FromBody] FawrypayRequest fawryPayRequest)
        {
            try
            {
                var apiUrl = _configuration.GetValue<string>("Fawry:URI");
                string response = await PostJson(apiUrl, fawryPayRequest);
                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making payment charge");
                return StatusCode(500, new { Message = "An error occurred while processing the payment.", Error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult GetChargeRefNumberStatus([FromBody] FawryPayStatusRequest fawryPayStatusRequest)
        {
            try
            {
                var apiUrl = _configuration.GetValue<string>("Fawry:StatusURI");
                string response = GetJsonStatus(apiUrl, fawryPayStatusRequest);
                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making payment charge");
                return StatusCode(500, new { Message = "An error occurred while processing the payment.", Error = ex.Message });
            }
        }

        public static string GenerateSignature(string merchantCode, string merchantRefNum, string customerName, string customerMobile, string customerEmail, string amount, string currencyCode, string signatureKey)
        {
            string signatureData = $"{merchantCode}{merchantRefNum}{customerName}{customerMobile}{customerEmail}{amount}{currencyCode}{signatureKey}";

            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(signatureData));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private async Task<string> PostJson(string uri, FawrypayRequest postParameters)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var json = JsonConvert.SerializeObject(postParameters);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        private static string GetJsonStatus(string uri, FawryPayStatusRequest getParameters)
        {
            string queryString = $"merchantCode={Uri.EscapeDataString(getParameters.merchantCode)}" +
                                 $"&merchantRefNumber={Uri.EscapeDataString(getParameters.merchantRefNumber)}" +
                                 $"&signature={Uri.EscapeDataString(getParameters.signature)}";

            uri += "?" + queryString;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json"; 

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
