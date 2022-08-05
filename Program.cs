using ModernBusinessContinuity.lang;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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
                var matchRegex = new Regex(!string.IsNullOrEmpty(config.only)? config.only: ".+\\.md", RegexOptions.IgnoreCase);

                if (!Directory.Exists(config.BuildDir)) Directory.CreateDirectory(config.BuildDir);

                foreach (var dir in dirs)
                {
                    if (Directory.Exists(dir))
                    {
                        string[] fileEntries = Directory.GetFiles(dir);
                        var vXYZ = new VerteXYZ();

                        foreach (var file in fileEntries)
                        {
                            if (matchRegex.IsMatch(file))
                            {
                                vXYZ.GenerateFile(file, config.BuildDir);
                            }
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
                Console.Write(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine);
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
        public string only { get; set; } = String.Empty;
    }
}
