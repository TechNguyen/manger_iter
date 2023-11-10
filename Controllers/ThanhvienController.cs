using System.Net;
using ClosedXML.Excel;
using It_Supporter.DataContext;
using It_Supporter.DataRes;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ThanhvienController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMember _member;
        private readonly IExcel _excel;
        private ThanhVienContext _context;
        public ThanhvienController(IMember member, ILogger<ThanhvienController> logger, ThanhVienContext context, IExcel excel)
        {
            _member = member;
            _logger = logger;
            _context = context;
            _excel = excel;
        }
        //layu danh sach tat ca tahnh vien
        [HttpGet("all")]
        public async Task<IActionResult> GetThanhViens([FromQuery] int page)
        {
            if (page < 1)
            {
                page = 1;
            }
            var data = _member.GetThanhViens(page);
            Console.WriteLine(data);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(data);
        }
        // lay danh sach thanh vien theo ma thanh vien
        [HttpGet("{mtv}")]
        [ProducesResponseType(200, Type = typeof(ThanhVien))]
        [ProducesResponseType(400)]
        public IActionResult GetMember(string mtv)
        {
            if (!_member.MemBerExit(mtv))
            {
                return NotFound();
            }
            else
            {
                var member = _member.GetMember(mtv);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(member);
            }
        }
        //lay danh sach theo khoa hoc
        [HttpGet("khoa/{khoahoc}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ThanhVien>))]
        [ProducesResponseType(400)]
        public IActionResult GetKhoa(string khoahoc)
        {
            var members = _member.GetThanhVienKhoa(khoahoc);
            if (members == null) {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(members);
        }


        //them moi 1 thanh vien
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMember([FromForm] ThanhVien thanhVien)
        {
            try {
                ProducerResCreate result = new ProducerResCreate();
                ThanhVien rs = await _member.CreateNewMember(thanhVien);
                if(rs != null) {
                    result.statuscode = 200;
                    result.message = "Create new member successfully!";
                    result.data = rs;
                }
                return Ok(result);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        // update thong tin thanh vien
        [HttpPut("update/{mtv}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public IActionResult UpdateMember(string mtv, [FromBody] ThanhVien thanhvien)
        {
            if (thanhvien == null)
            {
                return BadRequest(ModelState);
            }
            if (thanhvien.TenTv == null || thanhvien.Khoahoc == null || thanhvien.Nganhhoc == null || thanhvien.SoDT == null || thanhvien.DiaChi == null || thanhvien.Email == null || thanhvien.NgaySinh == null || thanhvien.Chucvu == null)
            {
                return NotFound();
            }
            if (!_member.UpdateMemberInfor(mtv, thanhvien))
            {
                ModelState.AddModelError("", "Not found member!");
                return NotFound(ModelState);
            }
            else
            {
                return Ok(_member.GetMember(mtv));
            }
        }
        //Xoa 1 thanh vien
        [HttpDelete("delete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult DeleteMember([FromQuery] string mtv)
        {
            if (mtv == null)
            {
                return BadRequest(ModelState);
            }
            if (!_member.DeleteMember(mtv))
            {
                ModelState.AddModelError("", "Not found memeber");
                return BadRequest(ModelState);
            }
            else
            {
                return Ok("Successfully Delete!");
            }
        }
        [HttpGet("dashboard")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        // count for admin
        public IActionResult ReportMember()
        {
            var result = _member.GetInforNumber();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(result);
        }
        // approve member to old member
        [Authorize(Roles = "Admin")]
        [HttpPost("approved")]
        public async Task<IActionResult> Approve([FromForm] string khoa) {
            try {
                var checkapprove = await _member.ApprovedMemberOld(khoa); 
                if(checkapprove) {
                    ApproveMemberRes approveMemberRes = new ApproveMemberRes {
                        statuscode = 200,
                        Message = "Approve member to old member succesfully!",
                    };
                    return Ok(approveMemberRes); 
                } else {
                    ApproveMemberRes approveMemberRes = new ApproveMemberRes {
                        statuscode = 400,
                        Message = "Error when approve member!"
                    };
                    return Ok(approveMemberRes); 
                }
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
        //approve member to tv
        [Authorize(Roles = "Admin")]
        [HttpPost("approve-member")]
        public async Task<IActionResult> ApproveNewMember([FromForm] string khoa) {
            try {
                var checkapprove = await _member.ApprovedNewMember(khoa); 
                if(checkapprove) {
                    ApproveMemberRes approveMemberRes = new ApproveMemberRes {
                        statuscode = 200,
                        Message = "Approve new member succesfully!",
                    };
                    return Ok(approveMemberRes); 
                } else {
                    ApproveMemberRes approveMemberRes = new ApproveMemberRes {
                        statuscode = 400,
                        Message = "Error when approve member!"
                    };
                    return Ok(approveMemberRes); 
                }
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("exportExcel")]
        // exppor data to excel
        public async Task<IActionResult> ExportToExcel() {
            try {
                var _datatable = _excel.ExportToExcel();
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.AddWorksheet(_datatable,"Members");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Member.xls");
                    }
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message); 
            }
        }


        [HttpPost("importExcel")]
        public async Task<IActionResult> ImportFileExcel(IFormFile file)
        {
            try
            {
                ProducerResponse producer = new ProducerResponse();
                var rs = await _excel.GenerrateExcel(file);
                if (rs)
                {
                    producer.statuscode = 200;
                    producer.message = "Successfully import data member!";
                } else
                {
                    producer.statuscode = 400;
                    producer.message = "Error when import data member";
                }
                return Ok(producer);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }


}
