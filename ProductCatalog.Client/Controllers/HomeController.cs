using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductCatalog.DAL.Entities;
using System.Net.Http.Headers;
using System.Text;

namespace ProductCatalog.Client.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7109/api");
        private readonly HttpClient _httpClient;

        public HomeController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        public async Task<IActionResult> Index()
        {
            var product = await GetProducts();
            if(product is null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            return View(product);
        }

        [HttpGet]
        public async Task<List<Product>> GetProducts()
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if(accessToken == null)
            {
                return (List<Product>)Results.Content("Access is available");

            }
            var url = "api/Product/GetProducts";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await _httpClient.GetStringAsync(url);

            var res = JsonConvert.DeserializeObject<List<Product>>(jsonStr).ToList();

            return res;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress +
                "/Auth/Register", content).Result;

            if(response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "New user";
                return RedirectToAction("/Login");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Login(UserDto user)
        {
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            using (var response = await _httpClient.PostAsync("api/Auth/Login", stringContent))
            {
                string token = await response.Content.ReadAsStringAsync();

                if (token == "Wrong password")
                {
                    ViewBag.Message = "Incorrect UserId or Password";
                    return Redirect("~/Home/Login");
                }
                HttpContext.Session.SetString("JWT", token);
            }
            return Redirect("~/Home/Index");

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Home/Login");
        }

        [HttpGet]
        public IActionResult CreateProducts()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProducts([Bind("Id, ProductName, ProductDescription, SpecialNote, Price, GeneralNote, CategoryId")] 
        Product product)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            var url = "api/Product/AddProducts";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var stringContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(url, stringContent);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            try
            {
                var accessToken = HttpContext.Session.GetString("JWT");
                if (accessToken == null)
                {
                    return (IActionResult)Results.Content("Access is available");

                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                Product product = new Product();
                HttpResponseMessage responseMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "/Product/GetProductById/" + id).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = responseMessage.Content.ReadAsStringAsync().Result;
                    product = JsonConvert.DeserializeObject<Product>(data);
                }
                return View(product);

    }
            catch (Exception ex)
            {
                TempData[""] = ex.Message;
                return View();
            }
            
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string data = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "/product/EditProducts", content).Result;

            if(httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            Product product = new Product();
            HttpResponseMessage responseMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "/Product/GetProductById/" + id).Result;

            if(responseMessage.IsSuccessStatusCode)
            {
                string data = responseMessage.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(data);
            }
            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        public IActionResult DeleteConfirmed(int id)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage responseMessage = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/Product/DeleteProducts/" + id).Result;

            if(responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}