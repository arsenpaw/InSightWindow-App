using Microsoft.Extensions.Logging;
using AXProductApp.Data;
using Microsoft.AspNetCore.SignalR;
using Blazored.LocalStorage;

namespace AXProductApp;

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


		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
        builder.Services.AddBlazoredLocalStorage();


        return builder.Build();
	}
}
