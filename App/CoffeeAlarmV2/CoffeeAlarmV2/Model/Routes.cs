using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeAlarmV2.Model
{
    public static class Routes
    {
        public const string SetPin = "/set";
        public const string Timer = "/timer";
        public const string SetCoffeeTimer = "/timer/coffee";
        public const string SetEspressoTimer = "/timer/espresso";
    }
}
