using It_Supporter.Models;

namespace It_Supporter.DataRes
{
    public class ProducerResManagerPosts : ProducerResponse
    {
        public List<inforPosts> listPosts { set; get; } 

        public int countPosts { set; get; }

    }
}
