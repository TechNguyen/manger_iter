using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;

namespace It_Supporter.DataRes
{
    public class ProducerResPost : ProducerResponse
    {
        public Posts data {set; get; }
    }
}