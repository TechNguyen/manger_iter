using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class DateSortPost
    {
        public DateTime? fromdate {set; get;} = DateTime.UtcNow;
        public DateTime? todate {set; get;} = DateTime.UtcNow;
    }
}