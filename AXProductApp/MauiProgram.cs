using Microsoft.Extensions.Logging;
using AXProductApp.Data;
using Microsoft.AspNetCore.SignalR;
using Blazored.LocalStorage;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.Maui.Audio;
using AXProductApp.Services;
using AXProductApp.Interfaces;

namespace AXProductApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseLocalNotification(config =>
            {
                config.AddAndroid(android =>
                {
                    android.AddChannel(new NotificationChannelRequest
                    {
                        Id = "alarm_sound1",
                        Name = "alarm_sound",   
                        Sound = "alarm_sound"
                    });
                });
            })
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<SendUserInputService>();
        builder.Services.AddSingleton<ReceiveWindowStatusService>();
        builder.Services.AddTransient<ILoginService, LoginService>();
        builder.Services.AddTransient<IRegisterService, RegisterService>();

        builder.Services.AddTransient<IMainMenu, MainMenuService>();
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();

        
        return builder.Build();
	}
}
