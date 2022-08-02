using System.Text;
using System.Text.Json;

namespace ModernBusinessContinuity
{
    public class Program
    {
        static Config config = new Config();
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string configFile;
            if (args.Length > 1)
            {
                configFile = args[0];
            }
            else
            {
                configFile = "config.json";
            }

            if (!File.Exists(configFile))
            {
                Console.WriteLine("Creating default config file");

                var writeNewConfig = JsonSerializer.Serialize<Config>(config);
                File.WriteAllText(configFile, writeNewConfig);
            }

            try
            {
                var jsonText = File.ReadAllText(configFile);
                config = JsonSerializer.Deserialize<Config>(jsonText) ?? new Config();

                var dirs = config.SourceDirs.Split(',', ';', ' ');
                config.BuildDir = Path.GetFullPath(config.BuildDir);

                if (!Directory.Exists(config.BuildDir)) Directory.CreateDirectory(config.BuildDir);

                foreach (var dir in dirs)
                {
                    if (Directory.Exists(config.SourceDirs))
                    {
                        string[] fileEntries = Directory.GetFiles(dir);
                        var vXYZ = new VerteXYZ();

                        foreach (var file in fileEntries)
                        {
                            vXYZ.GenerateFile(file, config.BuildDir);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Source Directory error: " + config.SourceDirs);
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Config
    {
        public string SourceDirs { get; set; } = "src/";
        public string BuildDir { get; set; } = "build/";
        public string main { get; set; } = "main.md";
    }
}
