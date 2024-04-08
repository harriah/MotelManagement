using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using TECH.Areas.Admin.Models;
using TECH.Areas.Admin.Models.Search;
using TECH.Service;
using System.Text.RegularExpressions;
using TECH.General;

namespace TECH.Areas.Admin.Controllers
{
    public class HopDongController : BaseController
    {
        private readonly IHopDongService _hopDongService;
        private readonly INhaService _nhaService;
        private readonly IPhongService _phongService;
        private readonly IDichVuPhongService _dichVuPhongService;
        private readonly IKhachHangService _khachHangService;
        private readonly INhanVienService _nhanVienService;
        private readonly IThanhVienPhongService _thanhVienPhongService;
        public HopDongController(IHopDongService hopDongService,
            INhaService nhaService,
            IPhongService phongService,
            IKhachHangService khachHangService,
            INhanVienService nhanVienService,
            IDichVuPhongService dichVuPhongService,
            IThanhVienPhongService thanhVienPhongService
            )
        {
            _hopDongService = hopDongService;
            _nhaService = nhaService;
            _phongService = phongService;
            _khachHangService = khachHangService;
            _nhanVienService = nhanVienService;
            _dichVuPhongService = dichVuPhongService;
            _thanhVienPhongService = thanhVienPhongService;
            //_nhaService = nhaService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddView()
        {
           
            return View();
        }

        [HttpGet]
        public JsonResult GetById(int id)
        {
            var model = new HopDongModelView();
            if (id > 0)
            {
                model = _hopDongService.GetByid(id);
            }
            return Json(new
            {
                Data = model
            });
        }

        [HttpPost]
        public JsonResult Add(HopDongModelView HopDongModelView)
        {
            _hopDongService.Add(HopDongModelView);
            // update trạng thái phòng thành đã có khách thuê
            if (HopDongModelView.MaPhong.HasValue && HopDongModelView.MaPhong.Value > 0 && HopDongModelView.TrangThai.HasValue && (HopDongModelView.TrangThai.Value == 1 || HopDongModelView.TrangThai.Value == 2))
            {
                _phongService.UpdateTrangThai(HopDongModelView.MaPhong.Value,2); // đã thuê
            }
            // Add thành viên phòng
            var thanhvienphong = new ThanhVienPhongModelView();
            thanhvienphong.MaKH = HopDongModelView.MaKH;
            thanhvienphong.MaPhong = HopDongModelView.MaPhong;
            _thanhVienPhongService.Add(thanhvienphong);
            _hopDongService.Save();
            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public JsonResult Update(HopDongModelView HopDongModelView)
        {
            var hopDongServer = new HopDongModelView();
            if (HopDongModelView != null && HopDongModelView.Id > 0)
            {
                hopDongServer = _hopDongService.GetByid(HopDongModelView.Id);
                if (hopDongServer != null && HopDongModelView != null)
                {
                    if (hopDongServer.MaPhong != HopDongModelView.MaPhong)
                    {
                        _phongService.UpdateTrangThai(hopDongServer.MaPhong.Value, 1); // Trống
                        _dichVuPhongService.DeletedByMaPhong(hopDongServer.MaPhong.Value);  // xóa dịch vụ của phòng
                        _thanhVienPhongService.DeletedByMaPhong(hopDongServer.MaPhong.Value);  // xóa thành viên phòng

                        _phongService.UpdateTrangThai(HopDongModelView.MaPhong.Value, 2); // đã thuê
                    }                  
                    if (hopDongServer.MaKH != HopDongModelView.MaKH)
                    {
                        // lấy thành viên cũ 
                        var thanhviencuphong = _thanhVienPhongService.GetByThanhVienByMaPhongMaKH(hopDongServer.MaKH.Value, hopDongServer.MaPhong.Value);
                        if (thanhviencuphong != null && thanhviencuphong.Id > 0)
                        {
                            _thanhVienPhongService.Deleted(thanhviencuphong.Id);
                            _thanhVienPhongService.Save();
                        }
                        var thanhvienphong = new ThanhVienPhongModelView();
                        thanhvienphong.MaKH = HopDongModelView.MaKH;
                        thanhvienphong.MaPhong = HopDongModelView.MaPhong;
                        _thanhVienPhongService.Add(thanhvienphong);
                    }
                }               
            }
            var result = _hopDongService.Update(HopDongModelView);
            if (HopDongModelView.TrangThai.HasValue && HopDongModelView.TrangThai.Value == 2)
            {
                _phongService.UpdateTrangThai(HopDongModelView.MaPhong.Value, 1); // Trống
                _dichVuPhongService.DeletedByMaPhong(HopDongModelView.MaPhong.Value);  // xóa dịch vụ của phòng
                _thanhVienPhongService.DeletedByMaPhong(HopDongModelView.MaPhong.Value);  // xóa thành viên phòng

            }else if (HopDongModelView.TrangThai.HasValue && HopDongModelView.TrangThai.Value == 1)
            {
                _phongService.UpdateTrangThai(HopDongModelView.MaPhong.Value, 2); // đã thuê
            }

            _hopDongService.Save();
            return Json(new
            {
                success = result
            });
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {
                var data = _hopDongService.GetByid(id);
                if (data != null && data.MaPhong.HasValue && data.MaPhong.Value > 0)
                {
                    _phongService.UpdateTrangThai(data.MaPhong.Value, 1); // Trống
                    _dichVuPhongService.DeletedByMaPhong(data.MaPhong.Value);  // xóa dịch vụ của phòng
                    _thanhVienPhongService.DeletedByMaPhong(data.MaPhong.Value);  // xóa thành viên phòng
                }
                 _hopDongService.UpdateIsDeteled(id,true);
                _hopDongService.Save();
                return Json(new
                {
                    success = true
                });
            }
            return Json(new
            {
                success = false
            });
        }

        [HttpGet]
        public JsonResult GetAllPaging(HopDongViewModelSearch phongViewModelSearch)
        {
            var data = _hopDongService.GetAllPaging(phongViewModelSearch);
            foreach (var item in data.Results)
            {
                if (item.MaNha.HasValue && item.MaNha.Value > 0)
                {
                    item.TenNha = _nhaService.GetByid(item.MaNha.Value)?.TenNha;
                }
                if (item.MaPhong.HasValue && item.MaPhong.Value > 0)
                {
                    item.TenPhong = _phongService.GetByid(item.MaPhong.Value)?.TenPhong;
                }
                if (item.MaKH.HasValue && item.MaKH.Value > 0)
                {
                    item.TenKhachHang = _khachHangService.GetByid(item.MaKH.Value)?.TenKH;
                }
                if (item.MaNV.HasValue && item.MaNV.Value > 0)
                {
                    item.TenNhanVien = _nhanVienService.GetByid(item.MaNV.Value)?.TenNV;
                }
                if (item.TrangThai.HasValue && item.TrangThai.Value > 0)
                {
                    item.TrangThaiStr = Common.GetTinhTrangHoaDon(item.TrangThai.Value);
                }
            }
            if (phongViewModelSearch != null && !string.IsNullOrEmpty(phongViewModelSearch.name))
            {
                data.Results = data.Results.Where(p => p.TenNha.Contains(phongViewModelSearch.name) ||
                p.TenPhong.Contains(phongViewModelSearch.name) ||
                p.TenKhachHang.Contains(phongViewModelSearch.name) ||
                p.TenNhanVien.Contains(phongViewModelSearch.name)).ToList();
            }
            if (phongViewModelSearch.status > 0)
            {
                data.Results = data.Results.Where(p=>p.TrangThai == phongViewModelSearch.status).ToList();
            }
            return Json(new { data = data });
        }

    }
}
