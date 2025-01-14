using Archive.ViewModels;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

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
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<Views.MoviesPage>();
            builder.Services.AddSingleton<ViewModels.MoviesViewModel>();
            builder.Services.AddSingleton<Views.TVSeriesPage>();
            builder.Services.AddSingleton<ViewModels.TVSeriesViewModel>();
            builder.Services.AddSingleton<Views.BooksPage>();
            builder.Services.AddSingleton<ViewModels.BooksViewModel>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
