using It_Supporter.Models;

namespace It_Supporter.DataRes
{
    public class ProducerResNotification : ProducerResponse
    {
        public List<Notification> notifications { get; set; }
    }
}
