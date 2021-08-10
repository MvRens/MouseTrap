using System;
using System.IO;
using Newtonsoft.Json;

namespace MouseTrap.Settings
{
    public class UserSettings
    {
        public bool EnableAtStartup
        {
            get => enableAtStartup;
            set
            {
                if (value == enableAtStartup)
                    return;

                enableAtStartup = value;
                Write();
            }
        }
        
        
        private readonly string filename;
        private bool enableAtStartup;



        public UserSettings()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MouseTrap");
            Directory.CreateDirectory(path);

            filename = Path.Combine(path, @"Settings.json");
        }


        public void Read()
        {
            if (!File.Exists(filename))
                return;

            var serializer = new JsonSerializer();

            using var streamReader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            using var jsonReader = new JsonTextReader(streamReader);
            
            var settings = serializer.Deserialize<SerializedSettings>(jsonReader);
            if (settings == null)
                return;

            enableAtStartup = settings.EnableAtStartup;
        }


        private void Write()
        {
            var settings = new SerializedSettings
            {
                EnableAtStartup = enableAtStartup
            };

            var serializer = new JsonSerializer { Formatting = Formatting.Indented };

            using var streamWriter = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));
            using var jsonWriter = new JsonTextWriter(streamWriter);
            
            serializer.Serialize(jsonWriter, settings);
        }


        private class SerializedSettings
        {
            public bool EnableAtStartup { get; set; }
        }
    }
}
