using System.Reflection;
using System.Text;
using AXProductApp.Data;
using AXProductApp.Interfaces;
using AXProductApp.Models.Configuration;
using AXProductApp.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Android;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Shared;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.Maui.Audio;

namespace AXProductApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "AXProductApp.appsettings.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        var jsonContent = reader.ReadToEnd();

        var aConfig = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonContent)))
            .Build();

        builder.Configuration.AddConfiguration(aConfig);

        
        builder.Configuration.AddConfiguration(aConfig);

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

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<SendUserInputService>();
        builder.Services.AddSingleton<ReceiveWindowStatusService>();
        builder.Services.AddTransient<ILoginService, LoginService>();
        builder.Services.AddTransient<IRegisterService, RegisterService>();
        builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddTransient<IMainMenu, MainMenuService>();
        builder.Services.AddTransient<IManageFireBaseTokenService, ManageFireBaseTokenService>();

        var baseUrl = aConfig["BaseUrl"];
        builder.Services.AddSingleton(provider => new AuthApiClient(baseUrl));

        builder.Logging.AddDebug();

        return builder.Build();
    }

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder, IConfiguration configuration)
    {
        var firebaseSettings = new FirebaseSettings();
        configuration.GetSection("FirebaseSettings").Bind(firebaseSettings);

        var isAuthEnabled = firebaseSettings.AuthEnabled;
        var isCloudMessagingEnabled = firebaseSettings.CloudMessagingEnabled;

        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddAndroid(android => android.OnCreate((activity, state) =>
                CrossFirebase.Initialize(activity, state, new CrossFirebaseSettings(
                    isAuthEnabled: isAuthEnabled,
                    isCloudMessagingEnabled: isCloudMessagingEnabled
                ))));
        });

        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        return builder;
    }
}