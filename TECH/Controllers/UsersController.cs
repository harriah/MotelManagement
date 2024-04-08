using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TECH.Areas.Admin.Models;
using TECH.Areas.Admin.Models.Search;
using System.Net.Mail;
using TECH.Service;

namespace TECH.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private readonly INhanVienService _nhanVienService;
        public IHttpContextAccessor _httpContextAccessor;
        public UsersController(INhanVienService nhanVienService,
            IHttpContextAccessor httpContextAccessor)
        {
            _nhanVienService = nhanVienService;
            _httpContextAccessor = httpContextAccessor;
        }  
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
           
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AppLogin(string userName, string passWord)
        {
            var result = _nhanVienService.AppUserLogin(userName, passWord);

            if (result != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("UserInfor", JsonConvert.SerializeObject(result));

                return Json(new
                {
                    success = result
                });
            }
            return Json(new
            {
                success = false
            });
        }


        //[HttpPost]
        //public JsonResult AddRegister(UserModelView UserModelView)
        //{
        //    bool isMailExist = false;
        //    bool isPhoneExist = false;
        //    if (UserModelView != null && !string.IsNullOrEmpty(UserModelView.email))
        //    {
        //        var isMail = _appUserService.IsMailExist(UserModelView.email);
        //        if (isMail)
        //        {
        //            isMailExist = true;
        //        }
        //    }

        //    if (UserModelView != null && !string.IsNullOrEmpty(UserModelView.phone_number))
        //    {
        //        var isPhone = _appUserService.IsPhoneExist(UserModelView.phone_number);
        //        if (isPhone)
        //        {
        //            isPhoneExist = true;
        //        }
        //    }

        //    if (!isMailExist && !isPhoneExist)
        //    {
        //        var result = _appUserService.Add(UserModelView);
        //        _appUserService.Save();
        //        if (result > 0)
        //        {
        //            var _user = _appUserService.GetByid(result);
        //            _httpContextAccessor.HttpContext.Session.SetString("UserInfor", JsonConvert.SerializeObject(_user));
        //        }
        //        return Json(new
        //        {
        //            success = result
        //        });
        //    }
        //    return Json(new
        //    {
        //        success = false,
        //        isMailExist = isMailExist,
        //        isPhoneExist = isPhoneExist
        //    });

        //}
        //public IActionResult Profile()
        //{
        //    var userString = _httpContextAccessor.HttpContext.Session.GetString("UserInfor");
        //    var user = new UserModelView();
        //    if (userString != null)
        //    {
        //        user = JsonConvert.DeserializeObject<UserModelView>(userString);
        //        if (!string.IsNullOrEmpty(user.address) && user.address != "null")
        //        {
        //            user.address = user.address;
        //        }
        //        else
        //        {
        //            user.address = "";
        //        }
        //        return View(user);
        //    }
        //    return Redirect("/home");

        //}

        //public JsonResult AppLogin(UserModelView UserModelView)
        //{
        //    var result = _appUserService.Add(UserModelView);
        //    _appUserService.Save();
        //    return Json(new
        //    {
        //        success = result
        //    });

        //}

        public IActionResult LogOut()
        {

            var userString = _httpContextAccessor.HttpContext.Session.GetString("UserInfor");
            var user = new UserModelView();
            if (userString != null)
            {
                user = JsonConvert.DeserializeObject<UserModelView>(userString);
                _httpContextAccessor.HttpContext.Session.Remove("UserInfor");
            }

            return Redirect("/home");

        }

        //public IActionResult ChangePass()
        //{
        //    var userString = _httpContextAccessor.HttpContext.Session.GetString("UserInfor");
        //    var model = new UserModelView();
        //    if (userString != null)
        //    {
        //        var user = JsonConvert.DeserializeObject<UserModelView>(userString);
        //        if (user != null)
        //        {
        //            var dataUser = _appUserService.GetByid(user.id);
        //            model = dataUser;
        //        }
        //        return View(model);
        //    }
        //    return Redirect("/home");
        //}
    }
}
