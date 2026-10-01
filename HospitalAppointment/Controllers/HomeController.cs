
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace HospitalAppointment.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Doctors");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Posts()
        {
            var client = _httpClientFactory.CreateClient();

            var posts = await client.GetFromJsonAsync<List<Post>>(
                "https://jsonplaceholder.typicode.com/posts");

            return View(posts ?? new List<Post>());
        }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
    }
}
