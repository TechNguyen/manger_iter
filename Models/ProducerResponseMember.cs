namespace It_Supporter.Models
{
    public class ProducerResponseMember
    {
        public int pageSize { get; set; }
        public int curentPage { get; set; }
        public int totalPage { get; set; }
        public int statuscode {get; set;}
        public string message {get; set;}
        public List<ThanhVien> data { get; set; }
    }
}
