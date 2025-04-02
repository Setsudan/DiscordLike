using System.Configuration;
using System.Data;
using System.Windows;
using DiscordLikeChatApp.Services;

namespace DiscordLikeChatApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static UserSession UserSession { get; } = new UserSession();
    public static ApiService ApiService { get; } = new ApiService();
    public static string ApiUrl { get; } = ConfigurationManager.AppSettings["ApiUrl"];
}

