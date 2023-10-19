using It_Supporter.DataRes;

namespace It_Supporter.Models
{
    public class ProducerResponseMember : ProducerResponse
    {
        public int pageSize { get; set; }
        public int curentPage { get; set; }
        public int totalPage { get; set; }
        public List<ThanhVien> data { get; set; }
    }
}
