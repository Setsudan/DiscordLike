using System;
using System.IO;
using DiscordHeticWpf.Models;
using Newtonsoft.Json;

namespace DiscordHeticWpf.Services
{
    public static class SessionManager
    {
        // File path in the user's AppData folder for persisting session information.
        private static readonly string SessionFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DiscordHeticWpf", "session.json");

        // Holds the current session (e.g., JWT token and user info).
        public static AuthData CurrentSession { get; private set; }

        // Returns true if a session is loaded and has a valid token.
        public static bool IsUserLoggedIn => CurrentSession != null && !string.IsNullOrEmpty(CurrentSession.Token);

        /// <summary>
        /// Saves the current session to a file.
        /// </summary>
        public static void SaveSession(AuthData session)
        {
            CurrentSession = session;
            try
            {
                var directory = Path.GetDirectoryName(SessionFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(SessionFilePath, JsonConvert.SerializeObject(session));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving session: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads the session from file, if it exists.
        /// </summary>
        public static void LoadSession()
        {
            try
            {
                if (File.Exists(SessionFilePath))
                {
                    var json = File.ReadAllText(SessionFilePath);
                    CurrentSession = JsonConvert.DeserializeObject<AuthData>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading session: " + ex.Message);
                CurrentSession = null;
            }
        }

        /// <summary>
        /// Clears the session from memory and deletes the session file.
        /// </summary>
        public static void ClearSession()
        {
            CurrentSession = null;
            try
            {
                if (File.Exists(SessionFilePath))
                {
                    File.Delete(SessionFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing session: " + ex.Message);
            }
        }
    }
}
