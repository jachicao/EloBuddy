using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using EloBuddy.Sandbox;
using EloBuddy.SDK.Events;

namespace LanguageTranslator
{
    internal static class Program
    {
        private static readonly string JsonPath = Path.Combine(ProgramDirectory, "Translations.json");
        private static readonly string ProgramDirectory = Path.Combine(SandboxConfig.DataDirectory, "LanguageTranslator");
        private const string VersionUrl = "https://raw.githubusercontent.com/jachicao/EloBuddy/master/LanguageTranslator/LanguageTranslator/Properties/AssemblyInfo.cs";
        private const string JsonUrl = "https://raw.githubusercontent.com/jachicao/EloBuddy/master/LanguageTranslator/LanguageTranslator/Translations.json";
        private const string VersionRegex = @"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]";
        private static WebClient _webClient;

        private static void Main()
        {
            Loading.OnLoadingComplete += delegate
            {
                Directory.CreateDirectory(ProgramDirectory);
                if (!File.Exists(JsonPath))
                {
                    File.Create(JsonPath).Close();
                    DownloadNewJson();
                }
                else
                {
                    _webClient = new WebClient();
                    _webClient.DownloadStringCompleted += VersionCompleted;
                    _webClient.DownloadStringAsync(new Uri(VersionUrl, UriKind.Absolute));
                }
            };
        }

        private static void VersionCompleted(object sender, DownloadStringCompletedEventArgs args)
        {
            _webClient.Dispose();
            _webClient = null;
            if (args.Cancelled || args.Error != null)
            {
                Console.WriteLine("Failed to download internet version.");
            }
            try
            {
                var match = Regex.Match(args.Result, VersionRegex);
                var internetVersion = Version.Parse(match.Groups[1].Value);
                var localVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (internetVersion > localVersion)
                {
                    DownloadNewJson();
                }
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        private static void DownloadNewJson()
        {
            _webClient = new WebClient();
            _webClient.DownloadStringCompleted += JsonDownloaded;
            _webClient.DownloadStringAsync(new Uri(JsonUrl, UriKind.Absolute));
        }

        private static void JsonDownloaded(object sender, DownloadStringCompletedEventArgs args)
        {
            _webClient.Dispose();
            _webClient = null;
            if (args.Cancelled || args.Error != null)
            {
                Console.WriteLine("Failed to download json file.");
            }
            File.WriteAllText(JsonPath, args.Result);
        }

        private static void OnLoad()
        {

        }
    }
}
