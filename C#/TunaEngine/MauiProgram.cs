using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace TunaEngine;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
#if WINDOWS
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(
			"SearchEntryBorderless",
			(handler, view) =>
			{
				if (view is Entry { AutomationId: "SearchEntry" })
				{
					handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
					handler.PlatformView.Background = null;
				}
			});
#endif

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
