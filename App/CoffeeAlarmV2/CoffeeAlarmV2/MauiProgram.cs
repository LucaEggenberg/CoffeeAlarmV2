﻿using CoffeeAlarmV2.Service;
using Microsoft.Extensions.Logging;

namespace CoffeeAlarmV2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Views
            builder.Services.AddTransient<MainPage>();

            // Viewmodels
            builder.Services.AddTransient<MainPageVM>();

            // Services
            builder.Services.AddTransient<ICoffeeService, CoffeeService>();
            builder.Services.AddTransient<IStorageService, StorageService>();

            return builder.Build();
        }
    }
}
