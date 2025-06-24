using Newtonsoft.Json;
using System.IO;

namespace RLoggerLib.Settings
{
    public class GlobalSettings
    {
        // Default settings file name
        private const string DEFAULT_SETTINGS_FILE_NAME = "rloggerlib-settings.json";

        public bool UseUtcTime { get; set; } = false;
        public bool ShowDate { get; set; } = true;
        public bool ShowMilliseconds { get; set; } = true;
        public bool ShowEventId { get; set; } = true;
        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        public GlobalSettings() { }

        // For internal cloning
        internal GlobalSettings(GlobalSettings settings)
        {
            UseUtcTime = settings.UseUtcTime;
        }

        /// <summary>
        /// Saves the settings to the default file.
        /// </summary>
        public void Save() => Save(DEFAULT_SETTINGS_FILE_NAME);

        /// <summary>
        /// Saves the settings to the specified file.
        /// </summary>
        /// <param name="filepath"> The file path to save the settings. </param>
        public void Save(string filepath)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(IOUtils.GetFullPath(filepath), json);
        }

        public static GlobalSettings Load() => Load(DEFAULT_SETTINGS_FILE_NAME);

        public static GlobalSettings Load(string filepath)
        {
            GlobalSettings? settings = null;
            string file = IOUtils.GetFullPath(filepath);
            var json = File.ReadAllText(file);
            settings = JsonConvert.DeserializeObject<GlobalSettings>(json);

            if (settings is null)
                throw new RLoggerLibException(MessageUtils.SETTINGS_FILE_EMPTY);

            return settings;
        }
    }
}
