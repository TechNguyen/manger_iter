using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class FileUpload
    {
        public IFormFile files {set; get;}
        public string fileName {set; get;}
    }
}