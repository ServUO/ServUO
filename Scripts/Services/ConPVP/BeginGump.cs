using System;
using Server.Gumps;

namespace Server.Engines.ConPVP
{
    public class BeginGump : Gump
    {
        private const int LabelColor32 = 0xFFFFFF;
        private const int BlackColor32 = 0x000008;
        public BeginGump(int count)
            : base(50, 50)
        {
            this.AddPage(0);

            const int offset = 50;

            this.AddBackground(1, 1, 398, 202 - offset, 3600);

            this.AddImageTiled(16, 15, 369, 173 - offset, 3604);
            this.AddAlphaRegion(16, 15, 369, 173 - offset);

            this.AddImage(215, -43, 0xEE40);

            this.AddHtml(22 - 1, 22, 294, 20, this.Color(this.Center("Duel Countdown"), BlackColor32), false, false);
            this.AddHtml(22 + 1, 22, 294, 20, this.Color(this.Center("Duel Countdown"), BlackColor32), false, false);
            this.AddHtml(22, 22 - 1, 294, 20, this.Color(this.Center("Duel Countdown"), BlackColor32), false, false);
            this.AddHtml(22, 22 + 1, 294, 20, this.Color(this.Center("Duel Countdown"), BlackColor32), false, false);
            this.AddHtml(22, 22, 294, 20, this.Color(this.Center("Duel Countdown"), LabelColor32), false, false);

            this.AddHtml(22 - 1, 50, 294, 80, this.Color("The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", BlackColor32), false, false);
            this.AddHtml(22 + 1, 50, 294, 80, this.Color("The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", BlackColor32), false, false);
            this.AddHtml(22, 50 - 1, 294, 80, this.Color("The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", BlackColor32), false, false);
            this.AddHtml(22, 50 + 1, 294, 80, this.Color("The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", BlackColor32), false, false);
            this.AddHtml(22, 50, 294, 80, this.Color("The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", 0xFFCC66), false, false);

            /*AddImageTiled( 32, 128, 264, 1, 9107 );
            AddImageTiled( 42, 130, 264, 1, 9157 );

            AddHtml( 60-1, 140, 250, 20, Color( String.Format( "Duel will begin in <BASEFONT COLOR=#{2:X6}>{0} <BASEFONT COLOR=#{2:X6}>second{1}.", count, count==1?"":"s", BlackColor32 ), BlackColor32 ), false, false );
            AddHtml( 60+1, 140, 250, 20, Color( String.Format( "Duel will begin in <BASEFONT COLOR=#{2:X6}>{0} <BASEFONT COLOR=#{2:X6}>second{1}.", count, count==1?"":"s", BlackColor32 ), BlackColor32 ), false, false );
            AddHtml( 60, 140-1, 250, 20, Color( String.Format( "Duel will begin in <BASEFONT COLOR=#{2:X6}>{0} <BASEFONT COLOR=#{2:X6}>second{1}.", count, count==1?"":"s", BlackColor32 ), BlackColor32 ), false, false );
            AddHtml( 60, 140+1, 250, 20, Color( String.Format( "Duel will begin in <BASEFONT COLOR=#{2:X6}>{0} <BASEFONT COLOR=#{2:X6}>second{1}.", count, count==1?"":"s", BlackColor32 ), BlackColor32 ), false, false );
            AddHtml( 60, 140, 250, 20, Color( String.Format( "Duel will begin in <BASEFONT COLOR=#FF6666>{0} <BASEFONT COLOR=#{2:X6}>second{1}.", count, count==1?"":"s", 0x66AACC ), 0x66AACC ), false, false );*/

            this.AddButton(314 - 50, 157 - offset, 247, 248, 1, GumpButtonType.Reply, 0);
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }
    }
}