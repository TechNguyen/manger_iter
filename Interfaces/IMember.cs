using It_Supporter.DataRes;
using It_Supporter.Models;
using System.Data;

namespace It_Supporter.Interfaces
{
    public interface IMember 
    {
        ProducerResponseMember GetThanhViens(int page);
        ThanhVien GetMember(string mtv);
        bool MemBerExit(string mtv);
        ICollection<ThanhVien> GetThanhVienKhoa(string khoahoc);
        Task<ThanhVien> CreateNewMember(ThanhVien thanhVien);
        bool UpdateMemberInfor(string mtv, ThanhVien thanhvien);
        bool DeleteMember(string mtv);
        Task<bool> ApprovedMemberOld(string khoa);
        Task<bool> ApprovedNewMember(string khoa);
        Task<ProducerResManagerMember> GetInforNumber(IConfiguration builder);
    }
}
