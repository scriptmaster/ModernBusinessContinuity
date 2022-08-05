using System.Text;

namespace ModernBusinessContinuity.lang
{
    public class GeneratedFile
    {
        public GeneratedFile(string directory, string fileName)
        {
            Directory = directory;
            FileName = fileName;
            Action = "create";
        }

        public GeneratedFile(string directory, string fileName, string action)
        {
            Directory = directory;
            FileName = fileName;
            Action = action;
        }

        public string Directory { get; }
        public string FileName { get; set; } = string.Empty;
        public string Action { get; set; } = "create";
        public StringBuilder Content { get; set; } = new StringBuilder();
        public string Include { get; set; } = string.Empty;
        public string First { get; set; } = string.Empty;
        public string Last { get; set; } = string.Empty;

        // public Dictionary<string, GeneratedFile> OtherFiles { get; set; } = 

        // public string Lang { get; set; } = "c"; // c18

        public void doAction(string genCode, string info)
        {
            if (!string.IsNullOrEmpty(info))
            {
                if (info == "include" || info == "#" || info == "i") // isInlude?
                {
                    // Include += (genCode.StartsWith("#") ? string.Empty : "#include ") + genCode + Environment.NewLine;
                    Include += genCode + Environment.NewLine;
                }
                if (info == "first") First += genCode + Environment.NewLine;
                if (info == "last") Last += genCode + Environment.NewLine;
                if (info != "c" && info != "raw") return;
            }

            if (Action == "create")
            {
                Content = new StringBuilder(1024); // 1KB
                Action = "append";
            }

            if (Action == "append")
            {
                Content.AppendLine(genCode);
            }
            else if (Action == "prepend")
            {
                Content.Insert(0, genCode);
            }
        }

        public void WriteAllText()
        {
            Content.Insert(0, Include + Environment.NewLine + (string.IsNullOrEmpty(First) ? string.Empty : First + Environment.NewLine));
            if (!string.IsNullOrEmpty(Last)) Content.Append(Environment.NewLine + Last);
            File.WriteAllText(Path.Join(Directory, FileName), Content.ToString());
        }
    }
}
