using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PS.Mobile.Pages;
using PS.Mobile.Services;
using PS.Mobile.Services.Interfaces;
using PS.Mobile.ViewModels;
using System.Net;

namespace PS.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
			// Initialize the .NET MAUI Community Toolkit by adding the below line of code
			.UseMauiCommunityToolkit()
				.ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);            
            //builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
            //builder.Services.AddSingleton<IMap>(Map.Default);

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IMemberService, MemberService>();            

            //  Pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<StationsPage>();
			builder.Services.AddSingleton<RegisterPage>();

			//   View Models
			builder.Services.AddSingleton<MainPageVM>();
            builder.Services.AddSingleton<LoginPageVM>();
            builder.Services.AddSingleton<StationsVM>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}