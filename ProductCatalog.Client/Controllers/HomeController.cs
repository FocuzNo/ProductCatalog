using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ProductCatalog.DAL.Entities;
using System.Net.Http.Headers;
using System.Security.Claims;
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

        #region


        public async Task<IActionResult> Index()
        {
            var product = await GetProducts();
            if (product is null)
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
            var res = JsonConvert.DeserializeObject<List<Product>>(jsonStr)!.ToList();

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

                if (token == "Wrong token")
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

        #endregion

        #region
        [HttpGet]
        public IActionResult CreateProducts()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProducts(Product product)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            string data = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = _httpClient.PostAsync(_httpClient.BaseAddress + "/Product/AddProducts", content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();

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
                    product = JsonConvert.DeserializeObject<Product>(data)!;
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
            HttpResponseMessage httpResponseMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "/Product/EditProducts", content).Result;

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
                product = JsonConvert.DeserializeObject<Product>(data)!;
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

        #endregion

        [HttpGet]
        public IActionResult CreateCategory()
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string data = JsonConvert.SerializeObject(category);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = _httpClient.PostAsync(_httpClient.BaseAddress + "/Category/AddCategory", content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
                var accessToken = HttpContext.Session.GetString("JWT");
                if (accessToken == null)
                {
                    return (IActionResult)Results.Content("Access is available");

                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                Category category = new Category();
                HttpResponseMessage responseMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "/Product/GetCategoryById/" + id).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = responseMessage.Content.ReadAsStringAsync().Result;
                    category = JsonConvert.DeserializeObject<Category>(data)!;
                }
                return View(category);

            }
        

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string data = JsonConvert.SerializeObject(category);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "/Category/EditCategory", content).Result;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public IActionResult DeleteCategory()
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return View();
        }

        [HttpPost, ActionName("DeleteCategory")]
        public IActionResult DeleteConfirmedCategory(int id)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage responseMessage = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/Category/DeleteCategory/" + id).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Panel()
        {
            var user = await GetUsers();
            if (user is null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            return View(user);
        }

        [HttpGet]
        public async Task<List<User>> GetUsers()
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (List<User>)Results.Content("Access is available");

            }

            var url = "api/User/GetUsers";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await _httpClient.GetStringAsync(url);

            var res = JsonConvert.DeserializeObject<List<User>>(jsonStr)!.ToList();

            return res;
        }

        [HttpGet]
        public IActionResult LoginForAdmin()
        {
            return View();
        }

        public async Task<IActionResult> LoginForAdmin(UserDto user)
        {
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync("api/Auth/Login", stringContent))
            {
                string token = await response.Content.ReadAsStringAsync();

                if (token == "Wrong token")
                {
                    ViewBag.Message = "Incorrect UserId or Password";
                    return Redirect("~/Home/Login");
                }
                HttpContext.Session.SetString("JWT", token);
            }


            return Redirect("~/Home/Panel");
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = _httpClient.PostAsync(_httpClient.BaseAddress + "/User/AddUser", content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Panel));
            }
            return View();
        }

        [HttpGet]
        public IActionResult BlockedUser()
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return View();
        }


        [HttpPost, ActionName("BlockedUser")]
        public IActionResult BlockedUserConfirmed(int id, User user)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "/User/BlockedUser/" + id, content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Panel));
            }
            return View();
        }

        [HttpGet]
        public IActionResult DeleteUser()
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return View();
        }

        [HttpPost, ActionName("DeleteUser")]
        public IActionResult DeleteConfirmedUser(int id)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage responseMessage = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/User/DeleteUser/" + id).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Panel));
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditPasswordUser()
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return View();
        }


        [HttpPost, ActionName("EditPasswordUser")]
        public IActionResult EditPassUserConfirmed(User user)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "/User/EditPassUser", content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Panel));
            }
            return View();
        }


        public async Task<IActionResult> TableForUser()
        {
            var product = await GetProductWithoutSpecial();
            if (product is null)
            {
                return (IActionResult)Results.Content("Access is available");
            }

            return View(product);
        }

        [HttpGet]
        public async Task<List<Product>> GetProductWithoutSpecial()
        {

            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (List<Product>)Results.Content("Access is available");
            }

            var url = "api/Product/GetProductWithoutSpecial";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonStr = await _httpClient.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<List<Product>>(jsonStr)!.ToList();

            return res;
        }

        [HttpGet]
        public IActionResult LoginForUser()
        {
            return View();
        }

        public async Task<IActionResult> LoginForUser(UserDto user)
        {
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync("api/Auth/Login", stringContent))
            {
                string token = await response.Content.ReadAsStringAsync();

                if (token == "Wrong token")
                {
                    ViewBag.Message = "Incorrect UserId or Password";
                    return Redirect("~/Home/LoginForUser");
                }
                HttpContext.Session.SetString("JWT", token);
            }


            return Redirect("~/Home/TableForUser");
        }

        public IActionResult CreateProductsForUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProductsForUser(Product product)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string data = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = _httpClient.PostAsync(_httpClient.BaseAddress + "/Product/AddProducts", content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(TableForUser));
            }
            return View();

        }

        [HttpGet]
        public IActionResult EditProductForUser(int id)
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
                    product = JsonConvert.DeserializeObject<Product>(data)!;
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
        public IActionResult EditProductForUser(Product product)
        {
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken == null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string data = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "/Product/EditProducts", content).Result;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(TableForUser));
            }
            return View();
        }
    }
}