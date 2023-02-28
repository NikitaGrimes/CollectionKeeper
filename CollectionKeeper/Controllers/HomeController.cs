using CollectionKeeper.Data;
using CollectionKeeper.Entities;
using CollectionKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SkiaSharp;
using System.Diagnostics;
using System.Text;

namespace CollectionKeeper.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<CollectionUser> _signInManager;

        private readonly UserManager<CollectionUser> _userManager;

        private readonly IApplicationRepository _applicationRepository;

        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(SignInManager<CollectionUser> signInManager, UserManager<CollectionUser> userManager,
            IApplicationRepository applicationRepository, IStringLocalizer<HomeController> localizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _applicationRepository = applicationRepository;
            _localizer = localizer;
        }

        [HttpGet]
        public string Search(string term)
        {
            IEnumerable<CollectionItem> items = _applicationRepository.GetTermsItems(term);
            StringBuilder result = new StringBuilder();
            foreach(var item in items)
            {
                result.Append("<a class=\"media-body text-decoration-none text-reset\" href=\"/Item/Detail/");
                result.Append(item.Id);
                result.Append("\"'> <p class=\"mb-1\"><strong>");
                result.Append(item.Name);
                result.Append("</strong></p> <p class=\"small text-muted\">");
                result.Append(item.CreatedDate); 
                result.Append("</p> </a> ");
            }

            return result.ToString();
        }

        public IActionResult Index()
        {
            LogoutAfterBlocked();
            HomeModel model = new HomeModel();
            model.Collections = _applicationRepository.GetBiggest3Collections().ToList();
            model.LastItems = _applicationRepository.GetLast5Items().ToList();
            IEnumerable<Tag> tags = _applicationRepository.GetAllTags().ToList();
            List<Tag> sortedTags = _applicationRepository.GetAllTags().OrderBy(t => t.Items.Count).ToList();
            int itemCount = _applicationRepository.GetAllItems().Count();
            List<TagModel> tagsModel = new List<TagModel>();
            foreach(var tag in tags)
            {
                tagsModel.Add(new TagModel() 
                { 
                    Id = tag.Id, 
                    Name = tag.Name,
                    CssClass = GetCssClass(tag, sortedTags)
                });
            }
            model.Tags = tagsModel;
            return View(model);
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public IActionResult SetTheme(string theme, string returnUrl)
        {
            Response.Cookies.Append( "theme", theme,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        private string GetCssClass(Tag tag, List<Tag> sortedTags)
        {
            int tagCount = sortedTags.Count();
            int tagNumber = sortedTags.IndexOf(tag);
            double result = (double)tagCount / 5;
            if (result >= tagNumber)
                return "TagSize5";
            if (result * 2 >= tagNumber)
                return "TagSize4";
            if (result * 3 >= tagNumber)
                return "TagSize3";
            if (result * 4 >= tagNumber)
                return "TagSize2";
            
            return "TagSize1";
        }

        [HttpGet]
        public IActionResult GetTagItems(int[] ids)
        {
            List<CollectionItem> items = _applicationRepository.GetTagItemsById(ids).ToList();
            List<CollectionItem> resultItems = new();
            foreach(var item in items)
            {
                bool isChecked = true;
                foreach(var id in ids)
                {
                    bool isFind = false;
                    foreach (var tag in item.Tags)
                    {
                        if (tag.Id == id)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        isChecked = false;
                        break;
                    }
                }
                if (isChecked)
                    resultItems.Add(item);
            }
            return PartialView("_Items", resultItems);
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                if (registerModel.Name.Trim() == String.Empty)
                {
                    ModelState.AddModelError("", _localizer["EmptyNameError"]);
                    return View();
                }

                CollectionUser user = new CollectionUser()
                {
                    UserName = registerModel.Name,
                    Email = registerModel.Email
                };
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result != IdentityResult.Success) 
                {
                    ModelState.AddModelError("", result.ToString());
                    return View();
                }

                return await Login(new LoginModel { Name= registerModel.Name, 
                    Password=registerModel.Password });
            }

            ModelState.AddModelError("", _localizer["FaildRegister"]);
            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (_applicationRepository.GetBlockUserStatus(loginModel.Name))
                {
                    ModelState.AddModelError("", _localizer["BlockError"]);
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(loginModel.Name, loginModel.Password,
                    false, false);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", _localizer["FaildLogin"]);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "admin")]
        public IActionResult Users()
        {
            if (LogoutAfterBlocked())
                return RedirectToAction("Index", "Home");

            return View(_applicationRepository.GetAllUsers().ToList());
        }

        [Authorize(Roles = "admin")]
        public IActionResult ManageUser(string id)
        {
            if (LogoutAfterBlocked())
                return RedirectToAction("Index", "Home");

            var user = _applicationRepository.GetUserById(id);
            if (user == null)
                return RedirectToAction("Users", "Home");

            return View(new ManageUserModel() { Id = user.Id, Email = user.Email,
            UserName = user.UserName, LockoutEnabled = user.LockoutEnabled});
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserRole(string id)
        {
            if (LogoutAfterBlocked())
                return RedirectToAction("Index", "Home");

            var user = _applicationRepository.GetUserById(id);
            if (user == null)
                RedirectToAction("Users", "Home");

            var role = await _userManager.GetRolesAsync(user);
            UserRoleModel model = new UserRoleModel();
            model.User = user;
            model.IsAdmin = role.Contains("admin");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserRole(UserRoleModel model)
        {
            if (LogoutAfterBlocked())
                return RedirectToAction("Index", "Home");

            var user = _applicationRepository.GetUserById(model.User.Id);
            if (user == null)
                return RedirectToAction("Users", "Home");

            if (model.IsAdmin)
                await _userManager.AddToRoleAsync(user, "admin");
            else
                await _userManager.RemoveFromRoleAsync(user, "admin");

            return RedirectToAction("Users", "Home");
        }

        [HttpPost]
        public IActionResult Delete(ManageUserModel _user)
        {
            if (LogoutAfterBlocked())
                return RedirectToAction("Index", "Home");

            var user = _applicationRepository.GetUserById(_user.Id);
            if (user != null)
                _applicationRepository.RemoveUser(user);

            return RedirectToAction("Users", "Home");
        }

        [HttpPost]
        public IActionResult Block(ManageUserModel _user)
        {
            if (LogoutAfterBlocked())
                return RedirectToAction("Index", "Home");

            if (_user == null)
                return RedirectToAction("Users", "Home");

            _applicationRepository.BlockUserById(_user.Id);
            return RedirectToAction("Users", "Home");
        }

        [HttpPost]
        public IActionResult UnBlock(ManageUserModel _user)
        {
            if (LogoutAfterBlocked())
                return RedirectToAction("Index", "Home");

            if (_user == null)
                return RedirectToAction("Users", "Home");

            _applicationRepository.UnBlockUserById(_user.Id);
            return RedirectToAction("Users", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool LogoutAfterBlocked()
        {
            if(User.Identity.IsAuthenticated && _applicationRepository.GetBlockUserStatus(User.Identity.Name))
            {
                _signInManager.SignOutAsync();
                return true;
            }

            return false;
        }
    }
}