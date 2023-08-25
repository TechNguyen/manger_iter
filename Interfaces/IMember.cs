using It_Supporter.Models;

namespace It_Supporter.Interfaces
{
    public interface IMember 
    {
        ProducerResponseMember GetThanhViens(int page);
        ThanhVien GetMember(string mtv);
        bool MemBerExit(string mtv);
        ICollection<ThanhVien> GetThanhVienKhoa(string khoahoc);
        bool CreateNewMember(string mtv, string tentv, string khoahoc, string nganhhoc, string sodt, string ngaysinh, string diachi, string chucvu, string emai, int namvaohoc);
        bool UpdateMemberInfor(string mtv, ThanhVien thanhvien);
        bool DeleteMember(string mtv);
        List<inforNumber> GetInforNumber();
    }
}
