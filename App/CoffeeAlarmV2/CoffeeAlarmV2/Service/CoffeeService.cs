using CoffeeAlarmV2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeAlarmV2.Service
{   
    public class CoffeeService : ICoffeeService
    {
        private const string ServiceUrl = "http://192.168.1.154:8000";
        private const int PinNumber = 17;

        private static HttpClient _httpClient = new()
        {
            BaseAddress = new Uri(ServiceUrl)
        };


        public async Task InitCoffeeMaker()
        {
            await TurnOn();
            await Task.Delay(20);
            await TurnOff();
        }

        public async Task TurnOn()
        {
            var response = await _httpClient.PatchAsync($"{Routes.SetPin}/{PinNumber}", GetSetBody(true));
            response.EnsureSuccessStatusCode();
        }

        public async Task TurnOff()
        {
            var response = await _httpClient.PatchAsync($"{Routes.SetPin}/{PinNumber}", GetSetBody(false));
            response.EnsureSuccessStatusCode();
        }

        public async Task<TimerResponse?> GetCurrentTimer()
        {
            return await _httpClient.GetFromJsonAsync<TimerResponse>(Routes.Timer);
        }

        public async Task SetCoffeeTimer(DateTime time)
        {
            var query = Routes.SetCoffeeTimer + GetQueryParameters("time", time.ToString("yyyy-MM-dd HH:mm:ss"));
            var response = await _httpClient.PutAsync(query, new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();
        }

        public async Task SetEspressoTimer(DateTime time)
        {
            var query = Routes.SetEspressoTimer + GetQueryParameters("time", time.ToString("yyyy-MM-dd HH:mm:ss"));
            var response = await _httpClient.PutAsync(query, new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTimer()
        {
            var response = await _httpClient.DeleteAsync(Routes.Timer);
            response.EnsureSuccessStatusCode();
        }

        private StringContent GetSetBody(bool isOn)
        {
            var stringContent = "{" + $"\"on\":{(isOn ? "true" : "false")}" + "}";
            return new StringContent(stringContent, Encoding.UTF8, "application/json");
        }

        private string GetQueryParameters(string key, string value)
        {
            return $"?{key}={value}";
        }
    }
}
