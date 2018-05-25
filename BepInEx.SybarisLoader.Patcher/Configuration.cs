using System;
using System.IO;
using System.Text;
using SimpleJSON;
using BepInEx.SybarisLoader.Patcher.Util;

namespace BepInEx.SybarisLoader.Patcher
{
    public static class Configuration
    {
        public static JSONNode Options { get; private set; }

        public static void Init()
        {
            string configFile = Path.Combine(Utils.SybarisDir, "SybarisLoader.json");

            if (!File.Exists(configFile))
            {
                InitDefaultConfig(configFile);
                return;
            }

            try
            {
                Options = JSON.Parse(File.ReadAllText(configFile, Encoding.UTF8));
            }
            catch (Exception)
            {
                Logger.Log(LogLevel.Warning, $"Failed to load configuration file {configFile}! Creating one!");
                InitDefaultConfig(configFile);
            }
        }

        private static void InitDefaultConfig(string path)
        {
            Options = new JSONObject();

            Options["debug"]["logging"]["enabled"] = false;
            Options["debug"]["logging"]["redirectConsole"] = true;
            Options["debug"]["logging"]["outputDirectory"] = Path.Combine(Utils.SybarisDir, "logs");

            StringBuilder sb = new StringBuilder();

            Options.WriteToStringBuilder(sb, 2, 2, JSONTextMode.Indent);

            try
            {
                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Warning, $"Failed to save configuration file to {path}!\nReason: {e.Message}");
            }
        }
    }
}