using CottageFinder.Interfaces;
using CottageFinder.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace CottageFinder
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
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();

            builder.Services.AddSingleton<ISensorsService, SensorsService>();
            builder.Services.AddSingleton<ILocationService, LocationService>();
            builder.Services.AddSingleton<IBearingService, BearingService>();
            builder.Services.AddSingleton<APIService>();
            builder.Services.AddSingleton<CottageStateService>();
            builder.Services.AddSingleton<Services.BlazorNavigationService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}



