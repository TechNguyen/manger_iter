using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using ExcelDataReader;
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
using NRedisStack.Literals.Enums;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;


namespace It_Supporter.Repository
{
    public class Member : IMember, IExcel 
    {
        private readonly ThanhVienContext _context;
        private readonly ICacheService _cacheService;
    

        private readonly Microsoft.Extensions.Hosting.IHostingEnvironment _env;
        public Member(ThanhVienContext context, ICacheService cacheService, Microsoft.Extensions.Hosting.IHostingEnvironment env)
        {
            _context = context;
            _cacheService = cacheService;
            _env = env;
        }

        public ProducerResponseMember GetThanhViens(int page)
        {
            var pageResult = 10f;
            var pageCount = Math.Ceiling(_context.THANHVIEN.Count() /  pageResult);
            var cacheDataMember = _cacheService.GetData<List<ThanhVien>>($"member/{page}");
            if(cacheDataMember != null && cacheDataMember.Count() > 0) {
                
                return new ProducerResponseMember
                {
                    statuscode = 200,
                    message = "Get member successfully!",
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
                statuscode = 200,
                message = "Get member succesfully!",
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
        public async Task<ThanhVien> CreateNewMember(ThanhVien thanhVien)
        {
            if (thanhVien.MaTV.Trim() == "" || thanhVien.TenTv.Trim() == "" || thanhVien.Khoahoc.Trim() == "" || thanhVien.Nganhhoc.Trim() == "" || thanhVien.SoDT.Trim() == "" || thanhVien.DiaChi.Trim() == "" || thanhVien.Chucvu.Trim() == "" || thanhVien.Email.Trim() == "")
            {
                return null;
            } else
            {
                if(thanhVien.deleted == null) {
                    thanhVien.deleted = 0;
                }
                var exprirationTime = DateTime.Now.AddMinutes(30);
                _context.THANHVIEN.Add(thanhVien);
                _cacheService.SetData<ThanhVien>($"member{thanhVien.MaTV}", thanhVien, exprirationTime);
                _context.SaveChangesAsync();
                return thanhVien;
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

        public async Task<bool> ApprovedMemberOld(string khoa)
        {
            try {
                var dsThanhVien = _context.THANHVIEN.Where(p => p.Khoahoc == khoa).ToList();
                foreach(var tv in dsThanhVien) {
                    tv.deleted = 1; 
                }
                _context.SaveChangesAsync();
                return true; 
            } catch (Exception ex) {
                return false;
            }
        }

        public async Task<bool> ApprovedNewMember(string khoa)
        {
            try {
                var dsThanhVien = _context.THANHVIEN.Where(p => p.Khoahoc == khoa).ToList();
                foreach(var tv in dsThanhVien) {
                    tv.Chucvu = "TV";
                }
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        public DataTable ExportToExcel()
        {
            DataTable dt = new DataTable();
            dt.TableName = "THANHVIEN";
            dt.Columns.Add("MaTV", typeof(string));
            dt.Columns.Add("TenTv", typeof(string));
            dt.Columns.Add("Khoahoc", typeof(string));
            dt.Columns.Add("Nganhhoc", typeof(string));
            dt.Columns.Add("SoDT", typeof(string));
            dt.Columns.Add("NgaySinh", typeof(DateTime));
            dt.Columns.Add("DiaChi", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("namvaohoc", typeof(int));
            dt.Columns.Add("Ban", typeof(string));
            var listMember = _context.THANHVIEN.Where(p => p.deleted == 0).ToList();

            if(listMember.Count > 0) {
                foreach (var member in listMember)
                {
                    dt.Rows.Add(member.MaTV,member.TenTv,member.Khoahoc,member.Nganhhoc,member.SoDT,member.NgaySinh,member.DiaChi,member.Email,member.namvaohoc,member.Ban);
                }
            }
            return dt;
        }

        public async Task<bool> GenerrateExcel(IFormFile file)
        {
            string rootpath = $"Upload\\Files";
            string pathfile = Path.Combine(rootpath, file.FileName);
            using (FileStream fs = new FileStream(Path.Combine(_env.ContentRootPath, pathfile), FileMode.Create))
            {
                file.CopyToAsync(fs);
                fs.FlushAsync();
            }
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(Path.Combine(_env.ContentRootPath, pathfile), FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var config = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };
                    var dataset = reader.AsDataSet(config);
                    if (dataset.Tables.Count > 0)
                    {
                        foreach (DataTable table in dataset.Tables)
                        {

                            for(int i = 1;i < table.Rows.Count;i++)
                            {

                                ThanhVien tv = new ThanhVien()
                                {
                                   MaTV = table.Rows[i]["MaTv"].ToString(),
                                   TenTv = table.Rows[i]["TenTv"].ToString(),
                                   Khoahoc = table.Rows[i]["Khoa"].ToString(),
                                   Nganhhoc = table.Rows[i]["Nganh"].ToString(),
                                   SoDT = table.Rows[i]["SoDT"].ToString(),
                                   NgaySinh = (DateTime)table.Rows[i]["NgaySinh"],
                                   DiaChi = table.Rows[i]["Diachi"].ToString(),
                                   Chucvu = table.Rows[i]["Chucvu"].ToString(),
                                   Email = table.Rows[i]["Email"].ToString(),
                                   namvaohoc =  int.Parse(table.Rows[i]["Nam"].ToString()),
                                   Ban = null,
                                   deleted = 0,
                                   urlImage = null
                                };
                                _context.THANHVIEN.Add(tv);
                            }
                        }

                    }
                }
            }
            return _context.SaveChanges() > 0 ? true : false;
        }
    }
}
