using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class ProducerResFormTech
    {
        public int countDevice {set; get;}
        public int completedDevice {set; get;}
        public int FixingDevice {set; get;}
        public IEnumerable<formTechUser> formTechUsers {set; get;} 
    }
}