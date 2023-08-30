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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var product = await GetProducts();
            if (product is null)
            {
                return (IActionResult)Results.Content("Access is available");
            }

            return View(product);
        }

        [HttpGet("Search")]
        public async Task <IActionResult> Index(string searchBy, string name)
        {
            if(searchBy is null || name is null)
            {
                var products = await GetProducts();
                return View(products);

            }
            var accessToken = HttpContext.Session.GetString("JWT");
            if (accessToken is null)
            {
                return (IActionResult)Results.Content("Access is available");

            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            List<Product> product = new List<Product>();
            HttpResponseMessage responseMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "/Product/SearchByProduct/" + searchBy + "/" + name).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                string data = responseMessage.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<List<Product>>(data)!;
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
        public IActionResult Register(UserDto user)
        {
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress +
                "/Auth/Register", content).Result;

            if(response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Успешная авторизация. Вы добавили нового пользователя. Переходите к аутентификации";
                return View();
            }

            TempData["errorMessage"] = "Неверные данные, либо пустые поля. Попробуйте еще раз.";

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

                HttpContext.Session.SetString("JWT", token);
            }

            TempData["successMessage"] = "Успешная аутентификация.";

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
                TempData["successMessage"] = "Продукты успешно добавлены.";
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не выбран Id категории.";
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

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Продукты успешно отредактированы.";
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не написан Id продукта.";
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

            if (responseMessage.IsSuccessStatusCode)
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

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Продукт успешно удален.";
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не написан Id продукта.";
            return View();
        }


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
                TempData["successMessage"] = "Категория успешно добавлены.";
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] = "Категория не была создана. Попробуйте еще раз.";
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
                TempData["successMessage"] = "Категория успешно отредактирована.";
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не указали Id.";
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
                TempData["successMessage"] = "Категория успешно удалена.";
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не указали Id.";
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
                HttpContext.Session.SetString("JWT", token);
            }

            TempData["successMessage"] = "Успешная аутентификация.";
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
                TempData["successMessage"] = "Успешно добавлен пользователь.";
                return RedirectToAction(nameof(Panel));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не указан логин или пароль.";
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
                TempData["successMessage"] = "Успешно пользователь заблокирован.";
                return RedirectToAction(nameof(Panel));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не указан Id пользователя.";
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
                TempData["successMessage"] = "Успешно пользователь удален.";
                return RedirectToAction(nameof(Panel));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не указан Id пользователя.";
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
                TempData["successMessage"] = "Успешно пароль пользователя отредактирован.";
                return RedirectToAction(nameof(Panel));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не указано имя пользователя.";
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

                HttpContext.Session.SetString("JWT", token);
            }

            TempData["successMessage"] = "Успешная аутентификация.";
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
                TempData["successMessage"] = "Успешно добавлен продукт.";
                return RedirectToAction(nameof(TableForUser));
            }

            TempData["successMessage"] = "Что-то пошло не так.";
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
                TempData["successMessage"] = "Успешно изменен продукт.";
                return RedirectToAction(nameof(TableForUser));
            }

            TempData["errorMessage"] = "Что-то пошло не так. Возможно не указан Id продукта.";
            return View();
        }
    }
}