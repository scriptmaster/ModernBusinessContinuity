using System.Text;
using System.Text.Json;

namespace ModernBusinessContinuity
{
    public class Program
    {
        static Config config;
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
                config = new Config();

                var writeNewConfig = JsonSerializer.Serialize<Config>(config);
                File.WriteAllText(configFile, writeNewConfig);
            }

            try
            {
                var jsonText = File.ReadAllText(configFile);
                config = JsonSerializer.Deserialize<Config>(jsonText) ?? new Config();

                if(Directory.Exists(config.SourceDirs))
                {
                    var dirs = config.SourceDirs.Split(',', ';', ' ');
                    foreach(var dir in dirs)
                    {
                        string[] fileEntries = Directory.GetFiles(dir);
                        var vXYZ = new VerteXYZ();

                        foreach (var file in fileEntries)
                        {
                            vXYZ.GenerateFile(file);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Directory does not exist: " + config.SourceDirs);
                }
            }
            catch(Exception ex)
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
        public string main { get; set; } = "முதல்.அ.md";
    }
}
