using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace ModernBusinessContinuity.lang
{
    // Proprietary LICENSE: https://github.com/scriptmaster/ModernBusinessContinuity/commits/master
    public class VerteXYZ
    {
        public string[] CodeLangTamil { get; set; } = new[] { "அ", "ல்", "த", "மி", "ழ்" };
        public string[] CodeLangArabic { get; set; } = new[] { "ع", "ا", "أ", "ل", "عربي" };
        public string[] CodeLangEnglish { get; set; } = new[] { "en" };

        private const string MAIN_C = "main.c";

        protected Dictionary<string, GeneratedFile> files = new();
        protected string buildDir = string.Empty;

        public void GenerateFile(string fromFile, string toDir)
        {
            if (!File.Exists(fromFile)) return;

            buildDir = toDir;
            var actionFileName = string.Empty;
            var codeLang = DetectCodeLang(fromFile);

            var fileContents = File.ReadAllText(fromFile);

            //var html = Markdown.ToHtml(fileContents);
            //Console.WriteLine("html: " + html);
            var extCreate = new Regex("^\\w+\\.(c|h|cs|java|html|js|json|css)$", RegexOptions.IgnoreCase);

            var parsed = Markdown.Parse(fileContents);
            foreach (var span in parsed)
            {
                var type = span.GetType().Name;
                switch (type)
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

                                if (extCreate.IsMatch(content))
                                {
                                    var cmd = content.Split(' ');
                                    var fileName = cmd[cmd.Length - 1];
                                    var action = cmd.Length > 1 ? cmd[0] : "append";

                                    actionFileName = fileName;

                                    var file = GetFile(actionFileName);
                                    file.Action = action;
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

                            if (string.IsNullOrEmpty(actionFileName)) actionFileName = MAIN_C;
                            var contentFile = GetFile(actionFileName);

                            if (isInclude)
                            {
                                var genCode = Include(codeLang, files[actionFileName], codeLine);
                                contentFile.doAction(genCode, "include");
                            }
                            else
                            {
                                var info = fencedCodeBlock.Info ?? string.Empty;
                                var genCode = fencedCodeBlock.Info == "c" ? codeLine : GenLang(codeLang, files[actionFileName], codeLine);
                                contentFile.doAction(genCode, fencedCodeBlock.Info ?? string.Empty);
                                genMetaFiles(codeLang, genCode, actionFileName);
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

        public GeneratedFile GetFile(string actionFileName)
        {
            if (string.IsNullOrEmpty(actionFileName) || !files.ContainsKey(actionFileName))
            {
                files[actionFileName] = new GeneratedFile(buildDir, actionFileName);
            }

            return files[actionFileName];
        }

        public GeneratedFile GetHeaderFile(string actionFileName)
        {
            var headerFileName = string.Concat(actionFileName.AsSpan(0, actionFileName.Length - 2), ".h");
            return GetFile(headerFileName);
        }

        // // // //
        List<string> pendingList = new List<string>() { };
        private void genMetaFiles(EveryIntrinsic codeLang, string codeLine, string actionFileName)
        {
            if (codeLine.StartsWith("//"))
            {
                // extra meta
                // var extraMeta = codeLine.Substring(2);
                var extraMetaRgx = new Regex("//(export|import)([: ](.+))?$");
                var extraMeta = extraMetaRgx.Match(codeLine);
                if (extraMeta.Success)
                {
                    if (extraMeta.Groups[1].Value == "export")
                    {
                        if (string.IsNullOrEmpty(extraMeta.Groups[3].Value.Trim()))
                        {
                            pendingList.Add("export");
                        }
                        else
                        {
                            // 
                        }
                    }
                    else if (extraMeta.Groups[1].Value == "import")
                    {
                        if (string.IsNullOrEmpty(extraMeta.Groups[3].Value.Trim()))
                        {
                            // Console.WriteLine("Empty import"); // could have been a user comment
                        }
                        else
                        {
                            if (actionFileName.EndsWith(".c"))
                            {
                                var headerFile = GetHeaderFile(actionFileName);
                                foreach (var imp in extraMeta.Groups[3].Value.Split(',', ' '))
                                {
                                    if (!string.IsNullOrEmpty(imp))
                                    {
                                        headerFile.doAction(Include(codeLang, headerFile, imp), "i");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (pendingList.Count > 0 && !string.IsNullOrEmpty(codeLine))
            {
                for(int i = pendingList.Count - 1; i >= 0; i--)
                {
                    var pL = pendingList[i];
                    if (pL == "export")
                    {
                        if (actionFileName.EndsWith(".c"))
                        {
                            codeLine.Trim(' ', '{');
                            var headerFile = GetHeaderFile(actionFileName);
                            headerFile.doAction(codeLine + ";", ""); //...
                        }
                        pendingList.RemoveAt(i);
                    }
                }
            }
        }

        public EveryIntrinsic DetectCodeLang(string fileName)
        {
            if (CodeLangTamil.Where(ext =>
                fileName.EndsWith("." + ext) ||
                    fileName.EndsWith(ext + ".md")
              ).Count() > 0) return ta.Intrinsic.Instance; // this.CodeLangTamil[0];
            if (CodeLangArabic.Where(ext =>
                fileName.EndsWith("." + ext) ||
                    fileName.EndsWith(ext + ".md")
              ).Count() > 0) return ar.Intrinsic.Instance; // this.CodeLangArabic[0];
            return en.Intrinsic.Instance;
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
}
