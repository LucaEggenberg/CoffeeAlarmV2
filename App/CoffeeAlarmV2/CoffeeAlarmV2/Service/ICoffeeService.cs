using CoffeeAlarmV2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeAlarmV2.Service
{
    public interface ICoffeeService
    {
        Task InitCoffeeMaker();
        Task TurnOn();
        Task TurnOff();
        Task<TimerResponse?> GetCurrentTimer();
        Task DeleteTimer();
        Task SetCoffeeTimer(DateTime time);
        Task SetEspressoTimer(DateTime time);
    }
}
