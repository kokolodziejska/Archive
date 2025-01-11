using Archive.ViewModels;
using Microsoft.Extensions.Logging;

namespace Archive
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
                    fonts.AddFont("Lato-Regular.ttf", "Lato");
                    fonts.AddFont("DeliusSwashCaps-Regular.ttf", "DeliusSwashCaps");
                });

            // Rejestracja ViewModel
            builder.Services.AddSingleton<Views.MoviesPage>();
            builder.Services.AddSingleton<ViewModels.MoviesViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
