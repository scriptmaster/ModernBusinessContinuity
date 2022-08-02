using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernBusinessContinuity.lang
{
    public abstract class EveryIntrinsic // base, belongs to longing being. :))
    {
        public Dictionary<string, Func<string>> keywords = new Dictionary<string, Func<string>>()
        {
            { "include:io", () => "#include <stdio.h>" },
            { "include:stdio", () => "#include <stdio.h>" },

            { "include:lib", () => "#include <stdlib.h>" },
            { "include:stdlib", () => "#include <stdlib.h>" },

            { "include:unistd", () => "#include <unistd.h>" },
            { "include:unix", () => "#include <unistd.h>" },

            //{ "include:iostream", () => "#include <iostream>" },//cpp//
            //Add java imports, C# usings etc.
        };

        //public EveryIntrinsic()
        //{
        //    // keywords.Clear();
        //    keywords.Add()
        //}

        internal void Info(string k, string w)
        {
            keywords.Add("info:" + k, () => { return w; });
        }

        internal void _include(string i, string? inc)
        {
            keywords["include:" + i] = () => "#include <"+ (inc ?? i) + ".h>";
        }

        internal void _include2(string i, string? inc)
        {
            keywords["include:" + i] = () => "#include \"" + (inc ?? i) + ".h\"";
        }

        internal void Standard(string io = "stdio", string lib = "stdlib")
        {
            _include(io, "stdio");
            _include(lib, "stdlib");
        }

        internal void POSIX(string unistd = "unistd")
        {
            _include(unistd, "unistd");
        }

        public string Include(string i)
        {
            var inc = i.ToLower().Replace("#", "").Replace("include", "").Replace("<", "").Replace(">", "").Replace("\"", "").Trim().TrimEnd('.', 'h');
            inc = inc.Split(' ')[0];
            if (string.IsNullOrEmpty(inc)) return String.Empty;
            if (keywords.ContainsKey("include:" + inc))
            {
                return keywords.ContainsKey("include:" + inc) ? keywords["include:" + inc]() : string.Empty;
            }
            else
            {
                Console.WriteLine("Include does not exist: " + inc);
                return string.Empty;
            }
        }

        public bool IsInclude(string? i)
        {
            if (string.IsNullOrEmpty(i)) return false;
            i = i.Split(' ')[0];
            return i == "#" || i == "i" || i == "inc" || i == "include";
        }


        internal void Funword(string funWord)
        {
            keywords.Add("funword:" + funWord, () => string.Empty);
        }

        internal void MoreFunwords()
        {
            // ["fun", "function", "proc", "procedure"];
            Funword("fun");
            Funword("function");
            Funword("proc");
            Funword("procedure");
        }
    }
}
