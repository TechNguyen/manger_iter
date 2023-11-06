using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Interfaces
{
    public interface IExcel
    {
        public DataTable ExportToExcel();
        Task<bool> GenerrateExcel(IFormFile file);
    }
}