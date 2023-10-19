using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;

namespace It_Supporter.DataRes
{
    public class ProducerPostManager : ProducerResponse
    {
        public int? counPosts {set; get;}

        public int? countdeletePost {set; get;}
        public ICollection<Posts> data {set; get;}

    }
}