using System;
using System.Collections.Generic;
using System.Text;

namespace Server.ACC.PG
{
    public partial class PGSystem 
    {
        /* PGAccessLevel:
         * Set this to whatever minimum AccessLevel you want to be able to  
         * modify this system via the Public Gate gumps.
         * NOTE: This has no effect on the ACC Main Gump settings.
         */
        public static AccessLevel PGAccessLevel = AccessLevel.GameMaster;
    }
}
