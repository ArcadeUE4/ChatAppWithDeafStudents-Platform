using ChatAppWithDeafStudents.Client.Helpers;
using ChatAppWithDeafStudents.Client.Pages;
using ChatAppWithDeafStudents.Client.Services;
using ChatAppWithDeafStudents.Client.ViewModel;
using ChatAppWithDeafStudents.Client.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ChatAppWithDeafStudents.Client
{
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

            // Загружаем конфигурацию
            var config = LoadConfiguration();
            var apiSettings = config.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();

            // Регистрируем конфигурацию в DI контейнере
            builder.Services.AddSingleton(apiSettings);

            // Конфигурируем HttpClient с базовым адресом из appsettings
            builder.Services.AddHttpClient<ServProvider>(client =>
            {
                client.BaseAddress = new Uri(apiSettings.BaseUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpHandler = new HttpClientHandler();
#if DEBUG
                httpHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
                return httpHandler;
            });

            builder.Services.AddSingleton<ChatHub>();

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginPageViewModel>();

            builder.Services.AddSingleton<ListChatPage>();
            builder.Services.AddSingleton<ListChatPageViewModel>();

            builder.Services.AddSingleton<ChatPage>();
            builder.Services.AddSingleton<ChatPageViewModel>();

            builder.Services.AddSingleton<AppShell>();

#if ANDROID
            builder.Services.AddSingleton<ISpeechToText, SpeechToTextImplementation>();
#endif

#if DEBUG
            builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Debug);
#else
            builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

            return builder.Build();
        }

        /// <summary>
        /// Loading configuration from appsettings.json
        /// </summary>
        private static IConfigurationRoot LoadConfiguration()
        {
            var assembly = typeof(MauiProgram).Assembly;
            using var stream = assembly.GetManifestResourceStream
                ("ChatAppWithDeafStudents.Client.appsettings.json");

            var configBuilder = new ConfigurationBuilder();

            if (stream != null)
            {
                configBuilder.AddJsonStream(stream);
            }

            return configBuilder.Build();
        }
    }
}
