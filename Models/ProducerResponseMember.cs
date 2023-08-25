namespace It_Supporter.Models
{
    public class ProducerResponseMember
    {
        public List<ThanhVien> thanhViens { get; set; }
        public int pageSize { get; set; }
        public int curentPage { get; set; }
        public int totalPage { get; set; }
    }
}
