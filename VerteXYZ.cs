using Markdig;
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
            Console.WriteLine("Generating file from: " + file);

            var fileContents = File.ReadAllText(file);
            var html = Markdown.ToHtml(fileContents);
            Console.WriteLine("html: " + html);

            var parsed = Markdown.Parse(fileContents);
            foreach(var span in parsed)
            {
                Console.WriteLine("Span: " + span);
            }
        }
        // // // // //
    }
}
