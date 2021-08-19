using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonalAppV2.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PersonalAppV2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public IActionResult Index() => View("Index2");
        
        [Route("GetEvents")]
        [HttpGet]
        public async Task<ActionResult> GetEvents()
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(HttpMethod.Get, "/home/");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                return Content(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Authorization");
            }

            throw new Exception("Problem accessing the API");
        }

        [Route("CreateEvent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult> CreateEvent([FromForm] CalendarEventCreate calendarEvent)
        {
            var uu = User;
            var httpClient = _httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(HttpMethod.Post, "/home/");
            var myContent = JsonSerializer.Serialize(calendarEvent);

            request.Content = new StringContent(
                myContent,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                return Content(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Authorization");
            }

            throw new Exception("Problem accessing the API");
        }

        [Route("DeleteEvent/{id:int}")]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(HttpMethod.Delete, "/home/"+id);

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                return Content(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Authorization");
            }

            throw new Exception("Problem accessing the API");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}
