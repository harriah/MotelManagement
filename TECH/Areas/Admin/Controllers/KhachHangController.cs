using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using TECH.Areas.Admin.Models;
using TECH.Areas.Admin.Models.Search;
using TECH.Service;

namespace TECH.Areas.Admin.Controllers
{
    public class KhachHangController : BaseController
    {
        private readonly IKhachHangService _khachhangService;
        public IHttpContextAccessor _httpContextAccessor;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        public KhachHangController(IKhachHangService nhanVienService,
             IHttpContextAccessor httpContextAccessor,
             Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _khachhangService = nhanVienService;
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetById(int id)
        {
            var model = new KhachHangModelView();
            if (id > 0)
            {
                model = _khachhangService.GetByid(id);
            }
            return Json(new
            {
                Data = model
            });
        }

        [HttpGet]
        public IActionResult AddOrUpdate()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Add(KhachHangModelView KhachHangModelView)
        {
            bool isMailExist = false;
            bool isPhoneExist = false;
            if (KhachHangModelView != null && !string.IsNullOrEmpty(KhachHangModelView.Email))
            {
                var isMail = _khachhangService.IsMailExist(KhachHangModelView.Email);
                if (isMail)
                {
                    isMailExist = true;
                }
            }

            if (KhachHangModelView != null && !string.IsNullOrEmpty(KhachHangModelView.SoDienThoai))
            {
                var isPhone = _khachhangService.IsPhoneExist(KhachHangModelView.SoDienThoai);
                if (isPhone)
                {
                    isPhoneExist = true;
                }
            }

            if (!isMailExist && !isPhoneExist)
            {
                _khachhangService.Add(KhachHangModelView);
                _khachhangService.Save();
                return Json(new
                {
                    success = true
                });
            }
            return Json(new
            {
                success = false,
                isMailExist = isMailExist,
                isPhoneExist = isPhoneExist
            });
        }

        [HttpPost]
        public JsonResult Update(KhachHangModelView KhachHangModelView)
        {
            var result = _khachhangService.Update(KhachHangModelView);
            _khachhangService.Save();
            return Json(new
            {
                success = result
            });

        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var data = _khachhangService.GetAll();
            return Json(new { Data = data });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var result = _khachhangService.Deleted(id);
            _khachhangService.Save();
            return Json(new
            {
                success = result
            });
        }

        [HttpGet]
        public JsonResult GetAllPaging(KhachHangViewModelSearch colorViewModelSearch)
        {
            var data = _khachhangService.GetAllPaging(colorViewModelSearch);
            return Json(new { data = data });
        }
    }
}
