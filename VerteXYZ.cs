using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModernBusinessContinuity
{
    // Propritary LICENSE: https://github.com/scriptmaster/ModernBusinessContinuity/commits/master
    public class VerteXYZ
    {
        public string[] CodeLangTamil { get; set; } = new[] { "அ", "த" };
        public string[] CodeLangArabic { get; set; } = new[] { "ع", "ا", "أ", "ل", "عربي" };
        public string[] CodeLangEnglish { get; set; } = new[] { "en" };

        private const string MAIN_FILE_NAME = "main.c";

        public void GenerateFile(string fromFile, string toDir)
        {
            if (!File.Exists(fromFile)) return;

            var files = new Dictionary<string, GeneratedFile>();
            var actionFileName = string.Empty;
            var codeLang = DetectCodeLang(fromFile); // set default by choosing "" in the code langs above;
            // can also be from configs or inference:

            var fileContents = File.ReadAllText(fromFile);
            // var fileDir = Path.GetDirectoryName(fromFile) ?? Path.GetFullPath(".");

            //var html = Markdown.ToHtml(fileContents);
            //Console.WriteLine("html: " + html);

            var parsed = Markdown.Parse(fileContents);
            foreach(var span in parsed)
            {
                var type = span.GetType().Name;
                switch(type)
                {
                    case "ParagraphBlock":
                        var block = span as LeafBlock ?? default; // segfault
                        if (block == null || block.Inline == null) continue;

                        foreach (var line in block.Inline)
                        {
                            if (line is CodeInline)
                            {
                                var content = (line as CodeInline)?.Content.Trim() ?? string.Empty;
                                Console.WriteLine("Check command: " + content);

                                if(content.EndsWith(".c") || content.EndsWith(".cs"))
                                {
                                    var cmd = content.Split(' ');
                                    var fileName = cmd[cmd.Length - 1];
                                    var action = cmd.Length > 1 ? cmd[0] : "append";

                                    if (!Regex.IsMatch(fileName, "^\\w+.c[s]?$")) continue;
                                    actionFileName = fileName;

                                    if (string.IsNullOrEmpty(actionFileName) || !files.ContainsKey(actionFileName))
                                    {
                                        files[fileName] = new GeneratedFile(toDir, fileName);
                                    }
                                    else
                                    {
                                        files[fileName].Action = action;
                                    }
                                }
                            }
                        }
                        break;
                    case "FencedCodeBlock":
                        var fencedCodeBlock = span as FencedCodeBlock ?? default; // segfault
                        if (fencedCodeBlock == null || fencedCodeBlock.Lines.Count <= 0) continue;

                        foreach (var line in fencedCodeBlock.Lines)
                        {
                            if (line == null || string.IsNullOrEmpty(line.ToString() ?? string.Empty)) continue;
                            var codeLine = (line.ToString() ?? string.Empty).Trim();

                            if (string.IsNullOrEmpty(codeLine)) continue;
                            Console.WriteLine("Code Line: " + codeLine);

                            if (string.IsNullOrEmpty(actionFileName) || !files.ContainsKey(actionFileName))
                            {
                                actionFileName = MAIN_FILE_NAME;
                                files[actionFileName] = new GeneratedFile(toDir, actionFileName); // if no file was specified
                            }

                            var genCode = ParseLang(codeLang, files[actionFileName], codeLine);
                            files[actionFileName].doAction(genCode);
                        }
                        break;
                }
            }

            foreach (var kv in files)
            {
                kv.Value.WriteAllText();
            }
        }

        public string DetectCodeLang(string fileName)
        {
            if (this.CodeLangTamil.Where(ext => 
                fileName.EndsWith("." + ext) ||
                    fileName.EndsWith(ext + ".md")
              ).Count() > 0) return this.CodeLangTamil[0];
            if (this.CodeLangArabic.Where(ext =>
                fileName.EndsWith("." + ext) || 
                    fileName.EndsWith(ext + ".md")
              ).Count() > 0) return this.CodeLangArabic[0];
            return string.Empty;
        }

        public string ParseLang(string fromLang, GeneratedFile toLangFileContext, string code)
        {
            if(this.CodeLangTamil.Contains(fromLang))
            {
                return code;
                // return $"//{this.CodeLangTamil[0]}: {code}";
            }
            else if (this.CodeLangArabic.Contains(fromLang))
            {
                return code;
                // return $"{code} //{this.CodeLangArabic[0]}";
            }
            else
            {
                return code;
            }
        }
    }

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

        // public string Lang { get; set; } = "c"; // c18


        public void doAction(string genCode)
        {
            if(Action == "create")
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
            File.WriteAllText(Path.Join(Directory, FileName), Content.ToString());
        }
    }
}
