using Microsoft.Extensions.Logging;
using AXProductApp.Data;
using Microsoft.AspNetCore.SignalR;
using Blazored.LocalStorage;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.Maui.Audio;
using AXProductApp.Services;
using AXProductApp.Interfaces;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Android;
using Plugin.Firebase.Auth;

using Plugin.Firebase.Shared;



namespace AXProductApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .RegisterFirebaseServices() 
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
        builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddTransient<IMainMenu, MainMenuService>();
        builder.Services.AddTransient<IManageFireBaseTokenService, ManageFireBaseTokenService>();
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddScoped<StateContainer>();
        builder.Logging.AddDebug();


        return builder.Build();

    }
    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events =>
        {

            events.AddAndroid(android => android.OnCreate((activity, state) =>
                CrossFirebase.Initialize(activity, state, CreateCrossFirebaseSettings())));
        });

        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        return builder;
    }

    private static CrossFirebaseSettings CreateCrossFirebaseSettings()
    {
        return new CrossFirebaseSettings(isAuthEnabled: true, isCloudMessagingEnabled: true);
    }
}
