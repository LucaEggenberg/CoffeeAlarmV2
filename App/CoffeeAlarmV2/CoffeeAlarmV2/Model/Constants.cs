using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeAlarmV2.Model
{
    public static class Constants
    {
        public const int AdminTapCount = 5;

        public const int TapDebounce = 500;

        public const string IsCoffeeStorageKey = "coffee_alarm_type";

        public const string SelectedTimeStorageKey = "coffee_alarm_time";
    }
}
