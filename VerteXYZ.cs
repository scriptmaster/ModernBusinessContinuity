﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using ModernBusinessContinuity.lang;

namespace ModernBusinessContinuity
{
    // Proprietary LICENSE: https://github.com/scriptmaster/ModernBusinessContinuity/commits/master
    public class VerteXYZ
    {
        public string[] CodeLangTamil { get; set; } = new[] { "அ", "ல்", "த", "மி", "ழ்"};
        public string[] CodeLangArabic { get; set; } = new[] { "ع", "ا", "أ", "ل", "عربي" };
        public string[] CodeLangEnglish { get; set; } = new[] { "en" };

        private const string MAIN_C = "main.c";

        public void GenerateFile(string fromFile, string toDir)
        {
            if (!File.Exists(fromFile)) return;

            var files = new Dictionary<string, GeneratedFile>();
            var actionFileName = string.Empty;
            var codeLang = DetectCodeLang(fromFile);

            var fileContents = File.ReadAllText(fromFile);

            //var html = Markdown.ToHtml(fileContents);
            //Console.WriteLine("html: " + html);
            var extCreate = new Regex("^\\w+\\.(c|h|cs|java|html|js|json|css)$", RegexOptions.IgnoreCase);

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

                                if(extCreate.IsMatch(content))
                                {
                                    var cmd = content.Split(' ');
                                    var fileName = cmd[cmd.Length - 1];
                                    var action = cmd.Length > 1 ? cmd[0] : "append";

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

                        var isInclude = codeLang.IsInclude(fencedCodeBlock.Info);

                        foreach (var line in fencedCodeBlock.Lines)
                        {
                            if (line == null || string.IsNullOrEmpty(line.ToString() ?? string.Empty)) continue;

                            var codeLine = line.ToString() ?? string.Empty;
                            if (string.IsNullOrEmpty(codeLine.Trim())) continue;

                            Console.WriteLine("FencedCodeBlock: " + codeLine);

                            if (string.IsNullOrEmpty(actionFileName) || !files.ContainsKey(actionFileName))
                            {
                                // if no file was specified, take main.c
                                actionFileName = MAIN_C;
                                files[actionFileName] = new GeneratedFile(toDir, actionFileName);
                            }

                            if(isInclude)
                            {
                                var genCode = Include(codeLang, files[actionFileName], codeLine);
                                files[actionFileName].doAction(genCode, "include");
                            }
                            else
                            {
                                var genCode = fencedCodeBlock.Info=="c"? codeLine: GenLang(codeLang, files[actionFileName], codeLine);
                                files[actionFileName].doAction(genCode, fencedCodeBlock.Info ?? string.Empty);
                            }
                        }
                        break;
                }
            }

            foreach (var kv in files)
            {
                kv.Value.WriteAllText();
            }
        }

        public EveryIntrinsic DetectCodeLang(string fileName)
        {
            if (this.CodeLangTamil.Where(ext =>
                fileName.EndsWith("." + ext) ||
                    fileName.EndsWith(ext + ".md")
              ).Count() > 0) return lang.ta.Intrinsic.Instance; // this.CodeLangTamil[0];
            if (this.CodeLangArabic.Where(ext =>
                fileName.EndsWith("." + ext) || 
                    fileName.EndsWith(ext + ".md")
              ).Count() > 0) return lang.ar.Intrinsic.Instance; // this.CodeLangArabic[0];
            return lang.en.Intrinsic.Instance;
        }

        public string GenLang(EveryIntrinsic fromLang, GeneratedFile toLangFileContext, string codeLine)
        {
            string gen = "//" + codeLine;
            // ...
            return gen;
        }

        public string Include(EveryIntrinsic fromLang, GeneratedFile toLangFileContext, string code)
        {
            string gen = fromLang.Include(code);
            if (string.IsNullOrEmpty(gen)) return "// " + code;
            return gen;
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
        public string Include { get; set; } = string.Empty;
        public string First { get; set; } = string.Empty;
        public string Last { get; set; } = string.Empty;

        // public string Lang { get; set; } = "c"; // c18

        public void doAction(string genCode, string info)
        {
            if(!string.IsNullOrEmpty(info))
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
            Content.Insert(0, Include + Environment.NewLine + (string.IsNullOrEmpty(First)? string.Empty: First + Environment.NewLine));
            if (!string.IsNullOrEmpty(Last)) Content.Append(Environment.NewLine + Last);
            File.WriteAllText(Path.Join(Directory, FileName), Content.ToString());
        }
    }
}
