using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernBusinessContinuity
{
    // Propritary LICENSE: https://github.com/scriptmaster/ModernBusinessContinuity/commits/master
    public class VerteXYZ
    {
        public string[] CodeLangTamil { get; set; } = new[] { "அ", "த", "" };
        public string[] CodeLangArabic { get; set; } = new[] { "ع", "ا", "عربي" };
        public string[] CodeLangEnglish { get; set; } = new[] { "en" };

        private string backfillFileName = "main.c";

        public void GenerateFile(string fromFile)
        {
            if (!File.Exists(fromFile)) return;

            Dictionary<string, GeneratedFile> files = new Dictionary<string, GeneratedFile>();
            string currentFileName = string.Empty;

            string codeLang = string.Empty; // set default by choosing "" in the code langs above; // can come from configs also.

            var fileContents = File.ReadAllText(fromFile);
            var fileDir = Path.GetDirectoryName(fromFile) ?? Path.GetFullPath(".");

            var html = Markdown.ToHtml(fileContents);
            Console.WriteLine("html: " + html);

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

                                if(content.EndsWith(".c"))
                                {
                                    var cmd = content.Split(' ');
                                    var fileName = cmd[cmd.Length - 1];
                                    var action = cmd.Length > 1 ? cmd[0] : "append";
                                    if (string.IsNullOrEmpty(currentFileName) || !files.ContainsKey(currentFileName))
                                    {
                                        files[fileName] = new GeneratedFile(fileDir, fileName);
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
                            if(string.IsNullOrEmpty(currentFileName) || !files.ContainsKey(currentFileName))
                            {
                                files[backfillFileName] = new GeneratedFile(fileDir, backfillFileName); // if no file was specified
                            }

                            var genCode = parseLang(codeLang, files[currentFileName], line?.ToString() ?? string.Empty);
                            files[currentFileName].doAction(genCode);

                            Console.WriteLine("Code Line: " + line.ToString());
                        }
                        break;
                }
            }
        }

        public string parseLang(string fromLang, GeneratedFile toLangFileContext, string code)
        {
            if(this.CodeLangTamil.Contains(fromLang))
            {
                return $"//{this.CodeLangTamil[0]}: {code}";
            }
            else if (this.CodeLangArabic.Contains(fromLang))
            {
                return $"//{this.CodeLangArabic[0]}: {code}";
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
        public string Content { get; set; } = string.Empty;

        // public string Lang { get; set; } = "c"; // c18


        public void doAction(string genCode)
        {
            if(Action == "create")
            {
                // if (File.Exists(FileName)) File.Move(FileName, );
                // overwrite than move - you can always regenerate on a different dir
                File.WriteAllText(FileName, string.Empty);
                Action = "append";
            }

            if (Action == "append")
            {
                File.AppendAllText(FileName, genCode);
                // it is okay to not use stream writer and close fd at every op. Can be optimized in vNext
            }
        }

    }
}
