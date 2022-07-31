using System.Text.Json;

namespace ModernBusinessContinuity
{
    public class Program
    {
        static Config config;
        static void Main(string[] args)
        {
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
            }
            catch(Exception ex)
            {
                // 
            }

            Console.ReadLine();
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
