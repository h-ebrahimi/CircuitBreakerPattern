using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CircuitBreakerPattern.Controller
{
    [ApiController]
    [Route("api/dogs")]
    public class DogsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public DogsController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("Dogs");
        }


        public async Task<IActionResult> GetDogs()
        {
            var message = string.Empty;
            try
            {
                using var requestMessage = new HttpRequestMessage();
                requestMessage.Method = new HttpMethod("GET");
                requestMessage.RequestUri = new System.Uri("https://dog.ceo/api/breeds/image/radom");
                var httpResponse = await _httpClient.SendAsync(requestMessage);
                message = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
                message = exp.Message + " " + DateTime.Now.ToString();
            }
            return Ok(message);
        }
    }
}