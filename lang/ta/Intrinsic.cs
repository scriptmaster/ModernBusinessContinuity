using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernBusinessContinuity.lang.ta
{
    public class Intrinsic : EveryIntrinsic
    {
        private static Intrinsic? instance = null;
        public static Intrinsic Instance { get { if (instance == null) instance = new Intrinsic(); return instance; } }

        public Intrinsic()
        {
            // for ```include
            Info("include", "சேர்");

            // map stdio and stdlib
            Standard("கோப்பு", "நூலகம்");
            //
            POSIX("லினக்ஸ்"); POSIX("யுனிக்ஸ்"); POSIX("போசிக்ஸ்"); POSIX("பொசிக்ஸ்");
            // ...
            Funword("செயல்"); // Keyword is Funword, Key opens, Fun continues
            // Enable English Function words
            EnFunwords();
        }
    }
}
