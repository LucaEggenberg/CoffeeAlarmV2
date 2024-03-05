using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeAlarmV2.Service
{
    public class StorageService : IStorageService
    {
        public void DeleteValue(string key)
        {
            var success = SecureStorage.Default.Remove(key);

            if (!success)
            {
                throw new Exception($"Could not delete Key {key}");
            }
        }

        public async Task<string?> GetValue(string key)
        {
            return await SecureStorage.Default.GetAsync(key);
        }

        public async Task SaveValue(string key, string value)
        {
            await SecureStorage.Default.SetAsync(key, value);
        }
    }
}
