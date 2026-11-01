using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TunaEngine.Services;

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
				if (view is Entry { AutomationId: "SearchEntry" or "LoginEntry" })
				{
					handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
					handler.PlatformView.Background = null;
				}
			});

		Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping(
			"LoginButtonBorderless",
			(handler, view) =>
			{
				if (view is Button
					{
						AutomationId: "LoginShowPasswordButton"
							or "LoginForgotPasswordButton"
							or "MemberEditButton"
							or "MemberRemoveButton"
					})
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

		builder.Services.AddSingleton(_ => new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(30)
		});
		builder.Services.AddSingleton<TunaEngineApiClient>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
