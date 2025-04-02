using System.Configuration;
using System.Data;
using System.Windows;
using DiscordLikeChatApp.Services;
using Microsoft.Extensions.Configuration;
using ConfigManager = System.Configuration.ConfigurationManager;

namespace DiscordLikeChatApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    public static UserSession UserSession { get; } = new UserSession();
    public static ApiService ApiService {
        get; private set;
    }
    public static string ApiUrl { get; } = ConfigManager.AppSettings["ApiUrl"];
    public static IConfiguration Configuration {
        get; private set;
    }
    public static string AuthToken {
        get; private set;
    }

    public App() {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Configuration = builder.Build();
    }

    public static void InitializeApiService(string authToken) {
        AuthToken = authToken;
        ApiService = new ApiService(Configuration, AuthToken);
    }
}
