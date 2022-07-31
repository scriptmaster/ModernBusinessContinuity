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
        // // // // //
        public static void GenerateFile(string file)
        {
            Console.OutputEncoding = Encoding.UTF8;
            // Console.WriteLine("Generating file from: " + file);

            var fileContents = File.ReadAllText(file);
            var html = Markdown.ToHtml(fileContents);
            LeafBlock? block;

            Console.WriteLine("html: " + html);

            var parsed = Markdown.Parse(fileContents);
            foreach(var span in parsed)
            {
                var type = span.GetType().Name;
                switch(type)
                {
                    case "ParagraphBlock":
                        block = span as LeafBlock ?? default; // segfault
                        if (block == null || block.Inline == null) continue;

                        foreach (var line in block.Inline)
                        {
                            if (line is CodeInline)
                            {
                                Console.WriteLine("Check command: " + line);
                            }
                        }
                        break;
                    case "FencedCodeBlock":
                        block = span as LeafBlock ?? default; // segfault
                        if (block == null || block.Inline == null) continue;

                        foreach (var line in block.Lines)
                        {
                            Console.WriteLine("Code Line: " + line);
                        }
                        break;
                }
            }
        }
        // // // // //
    }
}
