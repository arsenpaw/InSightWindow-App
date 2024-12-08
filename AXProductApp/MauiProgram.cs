using Android.App;
using Android.Content;
using AXProductApp.Data;
using AXProductApp.Interfaces;
using AXProductApp.Models.Configuration;
using AXProductApp.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.Firebase.CloudMessaging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.Maui.Audio;
using System.Reflection;
using Application = Android.App.Application;
using ILocalStorageService = AXProductApp.Services.ILocalStorageService;

namespace AXProductApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.AddGlobalExceptionHandler();

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "AXProductApp.appsettings.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);


        var aConfig = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();



        builder
            .RegisterFirebaseServices(aConfig)
            .UseMauiApp<App>()
            .UseLocalNotification(config =>
            {
                config.AddAndroid(android =>
                {
                    var notificationSetting = new NotificationSettings();
                    aConfig.GetSection("Notification").Bind(notificationSetting);
                    android.AddChannel(new NotificationChannelRequest
                    {
                        Id = notificationSetting.ChannelId,
                        Name = notificationSetting.ChannelName,
                        Sound = notificationSetting.Sound
                    });
                });
            })
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });
        builder.Configuration.AddConfiguration(aConfig);
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<SendUserInputService>();
        builder.Services.AddSingleton<ReceiveWindowStatusService>();
        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddTransient<ILocalStorageService, LocalStorageService>();
        builder.Services.AddTransient<IMainMenu, MainMenuService>();
        builder.Services.AddTransient<IManageFireBaseTokenService, ManageFireBaseTokenService>();
        builder.Services.AddSingleton<StateContainer>();
        var baseUrl = aConfig["BaseUrl"];
        builder.Services.AddSingleton<AuthApiClient>();

        builder.Logging.AddDebug();

        return builder.Build();
    }

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder, IConfiguration configuration)
    {
        var channelId = $"{Application.Context.PackageName}.general";
        var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);
        var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
        notificationManager.CreateNotificationChannel(channel);
        FirebaseCloudMessagingImplementation.ChannelId = channelId;
        return builder;
    }
}