using PS.Mobile.CustomComponents;

namespace PS.Mobile
{
    public partial class App : Application
    {
		public App()
		{
			InitializeComponent();

			MainPage = new AppShell();

			//	Replaces customr rendere in Xamarine forms, but for
			// Maui we now use Handler
			//	https://learn.microsoft.com/en-us/dotnet/maui/user-interface/handlers/
			//	https://learn.microsoft.com/en-us/dotnet/maui/user-interface/handlers/create
#pragma warning disable CA1416 // Validate platform compatibility
			Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
			{
				// We need to preform the mapping for android and IOS
#if __ANDROID__
				handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif __IOS__
				handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
				handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif

			});
#pragma warning restore CA1416 // Validate platform compatibility
		}

	}
}