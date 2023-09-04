using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Services
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expriseTime);
        object RemoveData(string key);
        Task<bool> RefreshData(string key);
    }
}