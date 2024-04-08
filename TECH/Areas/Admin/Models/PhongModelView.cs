using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TECH.Data.DatabaseEntity;

namespace TECH.Areas.Admin.Models
{
    public class PhongModelView
    {
        public int Id { get; set; }
        public int? MaNha { get; set; }
        public int? PhongTu { get; set; }
        public int? DenPhong { get; set; }
        public string? TenNha { get; set; }
        public string? TenPhong { get; set; }
        public decimal? DonGia { get; set; }
        public string? DonGiaStr { get; set; }
        public int? SLNguoiMax { get; set; }
        public int? ChieuDai { get; set; }
        public int? ChieuRong { get; set; }
        public string? MoTa { get; set; }
        public int? LoaiPhong { get; set; }
        public string? LoaiPhongStr { get; set; }
        public int? TinhTrang { get; set; }
        public string? TinhTrangStr { get; set; }

    }
}
