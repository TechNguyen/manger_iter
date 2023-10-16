using System.Net;
using It_Supporter.DataContext;
using It_Supporter.DataRes;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ThanhvienController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMember _member;
        private ThanhVienContext _context;
        public ThanhvienController(IMember member, ILogger<ThanhvienController> logger, ThanhVienContext context)
        {
            _member = member;
            _logger = logger;
            _context = context;
        }
        //layu danh sach tat ca tahnh vien
        [HttpGet]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ThanhVien>))]
        [Produces("application/json")]

        public IActionResult GetThanhViens([FromQuery] int page)
        {
            if (page < 1)
            {
                page = 1;
            }
            var data = _member.GetThanhViens(page);
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
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateMember([FromForm] ThanhVien thanhVien)
        {
            try {
                var rs = await _member.CreateNewMember(thanhVien);
                return Ok(rs);
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
                var checkapprove = await _member.ApprovedMemberOld(khoa); 
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

    }
}
