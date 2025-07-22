using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls; // Ajoute ce using

namespace MauiApp1
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
                })
                .UseBarcodeReader(); // <-- Ajoute cette ligne

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}