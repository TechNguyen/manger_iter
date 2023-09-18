using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using It_Supporter.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;

namespace It_Supporter.Repository
{
    public class Member : IMember
    {
        private readonly ThanhVienContext _context;
        private readonly ICacheService _cacheService;
    
        public Member(ThanhVienContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public ProducerResponseMember GetThanhViens(int page)
        {
            var pageResult = 10f;
            var pageCount = Math.Ceiling(_context.THANHVIEN.Count() /  pageResult);


            var cacheDataMember = _cacheService.GetData<List<ThanhVien>>($"member/{page}");

            if(cacheDataMember != null && cacheDataMember.Count() > 0) {
                
                return new ProducerResponseMember
                {
                    data = cacheDataMember,
                    curentPage = page,
                    pageSize = (int)pageResult,
                    totalPage = (int)pageCount
                };
            }
            var Members = _context.THANHVIEN.Skip((page - 1) * (int)pageResult)
                                            .Take((int)pageResult)
                                            .ToList();
            var expraiseTime = DateTime.Now.AddMinutes(30);
            _cacheService.SetData<List<ThanhVien>>($"member/{page}", Members, expraiseTime);
            var response = new ProducerResponseMember
            {
                data = Members,
                curentPage = page,
                pageSize = (int)pageResult,
                totalPage = (int)pageCount
            };
            return response;
        }
        // tim kiem thanh vien
        public ThanhVien GetMember(string mtv)
        {
            return _context.THANHVIEN.Where(p => p.MaTV == mtv).FirstOrDefault();
        }
        // tiim kiiem theo khoa
        public ICollection<ThanhVien> GetThanhVienKhoa(string khoahoc)
        {
            var cacheData = _cacheService.GetData<ICollection<ThanhVien>>("member");
            if(cacheData != null && cacheData.Count() > 0) {
                return cacheData;
            }
            //caching redis memmber
            cacheData = _context.THANHVIEN.Where(p => p.Khoahoc == $"K{khoahoc}").ToList();
            //set exsprisetime
            var expriseTime = DateTimeOffset.Now.AddMinutes(30);
            _cacheService.SetData<ICollection<ThanhVien>>($"member{khoahoc}", cacheData, expriseTime);
            return cacheData; 

        }
        //tao 1 thanh vien
        public bool CreateNewMember(string mtv, string tentv, string khoahoc, string nganhhoc, string sodt, string ngaysinh, string diachi, string chucvu, string email, int namvaohoc)
        {
            if (mtv == "" || tentv == "" || khoahoc == "" || nganhhoc == "" || ngaysinh == "" || sodt == "" || diachi == "" || chucvu == "" || email == "")
            {
                return false;
            } else
            {
                var datetime = DateTime.Parse(ngaysinh);
                var newMember = new ThanhVien()
                {
                    MaTV = mtv,
                    TenTv = tentv,
                    Khoahoc = khoahoc,
                    Nganhhoc = nganhhoc,
                    SoDT = sodt,
                    NgaySinh = datetime,
                    DiaChi = diachi,
                    Chucvu = chucvu,
                    Email = email,
                    namvaohoc = namvaohoc
                };

                var exprirationTime = DateTime.Now.AddMinutes(30);
                _context.THANHVIEN.Add(newMember);
                _cacheService.SetData<ThanhVien>($"member{newMember.MaTV}", newMember, exprirationTime);
                _context.SaveChanges();
                return true;
            }
        }


        //Update thong tin
        public bool UpdateMemberInfor(string mtv, ThanhVien thanhvien)
        {
            var _member = _context.THANHVIEN.FirstOrDefault(p => p.MaTV == mtv);
            if (_member == null)
            {
                return false;
            }
            else
            {
                _member.MaTV = thanhvien.MaTV;
                _member.TenTv = thanhvien.TenTv;
                _member.Khoahoc = thanhvien.Khoahoc;
                _member.Nganhhoc = thanhvien.Nganhhoc;
                _member.SoDT = thanhvien.SoDT;
                _member.NgaySinh = thanhvien.NgaySinh;
                _member.DiaChi = thanhvien.DiaChi;
                _member.Chucvu = thanhvien.Chucvu;
                _member.Email = thanhvien.Email;
                _member.namvaohoc = thanhvien.namvaohoc;
                _context.SaveChanges();
                return true;
            }
        }

        //xxoa 1 thanh vien
        public bool DeleteMember(string mtv)
        {
            ThanhVien _thanhvien = _context.THANHVIEN.FirstOrDefault(p => p.MaTV == mtv);
            if(_thanhvien  == null)
            {
                return false;
            } else
            {
                _context.THANHVIEN.Remove(_thanhvien);
                _cacheService.RemoveData($"member{mtv}");
                _context.SaveChanges();
                return true;
            }
        }

        // lay ra thong so cho admin
        public List<inforNumber> GetInforNumber()
        {
            var result = new List<inforNumber>();
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Data Source=LAPTOP-32QQSE7C\\SQLEXPRESS;Initial Catalog=ITSUPPPORTER;Persist Security Info=True;User ID=sa;Password=nguyenthang1306;TrustServerCertificate=true";
            connection.Open();
            string procedureName = "dbo.SP_Count_Member";
            var countParams = new SqlParameter("@counthanhvien", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            var firstyear = new SqlParameter("@firstyearthanhvien", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            var secondyear = new SqlParameter("@secondyearthanhvien", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            var thirdyear = new SqlParameter("@thirdyearthanhvien", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            using (SqlCommand command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(countParams);
                command.Parameters.Add(firstyear);
                command.Parameters.Add(secondyear);
                command.Parameters.Add(thirdyear);

                command.ExecuteNonQuery();
                int outputValue = (int)command.Parameters["@counthanhvien"].Value;
                int firtValue = (int)command.Parameters["@firstyearthanhvien"].Value;
                int secondValue = (int)command.Parameters["@secondyearthanhvien"].Value;
                int thirdValue = (int)command.Parameters["@thirdyearthanhvien"].Value;

                inforNumber tmpRecord = new inforNumber()
                {
                    CountMember = outputValue,
                    firstyearMember = firtValue,
                    secondyearMember = secondValue,
                    thirdyearMember = thirdValue,   
                };
                result.Add(tmpRecord);
            }
            return result;
            
        }

        public bool MemBerExit(string mtv)
        {
            return _context.THANHVIEN.Any(p => p.MaTV == mtv);
        }
    }
}
