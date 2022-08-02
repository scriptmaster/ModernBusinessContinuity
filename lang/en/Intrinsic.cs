using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernBusinessContinuity.lang.en
{
    public class Intrinsic : EveryIntrinsic
    {
        private static Intrinsic? instance = null;
        public static Intrinsic Instance { get { if (instance == null) instance = new Intrinsic(); return instance; } }

        public Intrinsic()
        {
            // for ```include
            //Info("include", "");
            // map stdio and stdlib
            //Standard("", "");
            //POSIX("");
            // ...
            //Funword(""); // Keyword is Funword, Key opens, Fun continues
        }
    }
}
