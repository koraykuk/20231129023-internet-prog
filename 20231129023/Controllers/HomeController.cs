using _20231129023.Models;
using _20231129023.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt.Extensions;
using System.Diagnostics;
using System.Security.Claims;

namespace _20231129023.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly INotyfService _notyf;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger,AppDbContext context,IConfiguration config, INotyfService notyf)
        {
            _logger = logger;
            _context = context;
            _config = config;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Admin()
        {
            var questions = _context.Questions
                .Select(q => new Question
                {
                    Id = q.Id,
                    UserId = q.UserId,
                    QuestionTitle = q.QuestionTitle,
                    QuestionText = q.QuestionText,
                    Date = q.Date
                })
                .AsEnumerable<Question>();

            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (_context.Users.Where(s => s.UserName == model.UserName).Count() > 0)
            {
                _notyf.Error("Girilen Kullanıcı Adı Kayıtlıdır!");
                return View(model);
            }
            
            var user = new User();
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Password = MD5Hash(model.Password);
            user.Role = "Üye";
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _notyf.Success("Üye Kaydı Yapılmıştır. Oturum Açınız");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            var hashedpass = MD5Hash(model.Password);
            var user = _context.Users.Where(s => s.UserName == model.UserName && s.Password == model.Password).SingleOrDefault();

            if (user == null)
            {
                _notyf.Error("Kullanıcı Adı veya Parola Geçersizdir!");
                return Redirect("Index");
            }

            List<Claim> claims = new List<Claim>() {

                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim("UserName",user.UserName),
                new Claim("Email",user.Email)
                };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = model.KeepMe
            };
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

            if(user.Role == "admin")
            {
                return RedirectToAction("Admin");
            }
            else if (user.Role == "Üye")
            {
                return Redirect("/Questions");
            }
            _notyf.Error("Kullanıcı Adı veya Parola Geçersizdir!");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string MD5Hash(string pass)
        {
            var salt = _config.GetValue<string>("AppSettings:MD5Salt");
            var password = pass + salt;
            var hashed = password.MD5();
            return hashed;
        }

    }
}
