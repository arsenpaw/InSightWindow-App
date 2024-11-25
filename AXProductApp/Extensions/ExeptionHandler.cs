namespace AXProductApp;

public static class ExeptionHandler
{
    public static void AddGlobalExceptionHandler(this MauiAppBuilder builder)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var exception = e.ExceptionObject as Exception;
            HandleException(exception, "Unhandled Exception");
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            e.SetObserved();
            HandleException(e.Exception, "Unobserved Task Exception");
        };
    }

    private static void HandleException(Exception exception, string source)
    {
        Console.WriteLine($"[{source}] {exception?.Message}\n{exception?.StackTrace}");

        Application.Current?.MainPage?.Dispatcher.Dispatch(async () =>
        {
            await Application.Current.MainPage.DisplayAlert(
                "CRITICAL!",
                $"Something went wrong: {exception?.Message}",
                "OK"
            );
        });
    }
}