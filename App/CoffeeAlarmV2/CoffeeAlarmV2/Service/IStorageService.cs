using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeAlarmV2.Service
{
    public interface IStorageService
    {
        Task SaveValue(string key, string value);
        void DeleteValue(string key);
        Task<string?> GetValue(string key);
    }
}
