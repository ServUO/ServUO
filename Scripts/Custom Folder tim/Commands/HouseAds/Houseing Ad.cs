// by Old School Oct 2008
#define RunUo2_0

using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;

namespace Server.Gumps
{
    public class UOHouseingAd : Gump
    {
        Mobile caller;

        public static void Initialize()
        {
#if(RunUo2_0)
            CommandSystem.Register("HouseingAd", AccessLevel.Player, new CommandEventHandler(HouseingAd_OnCommand));
#else
            Register("HouseingAd", AccessLevel.Player, new CommandEventHandler(HouseingAd_OnCommand));
#endif
        }

        [Usage("HouseingAd")]
        [Description("Makes a call to your custom gump.")]
        public static void HouseingAd_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from.HasGump(typeof(UOHouseingAd)))
                from.CloseGump(typeof(UOHouseingAd));
            from.SendGump(new UOHouseingAd(from));
        }

        public UOHouseingAd(Mobile from) : this()
        {
            caller = from;
        }

        public UOHouseingAd() : base( 0, 0 )
        {
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=true;

			AddPage(0);
			AddImageTiled(191, 57, 448, 508, 5174);
			AddImage(174, 21, 5150);
			AddImage(212, 20, 5171);
			AddImage(382, 20, 5171);
			AddImage(618, 20, 5152);
			AddImage(448, 20, 5171);
			AddImage(174, 57, 5153);
			AddImage(174, 167, 5173);
			AddImage(174, 276, 5153);
			AddImage(174, 389, 5153);
			AddImage(174, 452, 5173);
			AddImage(173, 524, 5176);
			AddImage(211, 524, 5177);
			AddImage(381, 524, 5177);
			AddImage(469, 524, 5177);
			AddImage(618, 524, 5178);
			AddLabel(306, 49, 1357, @"Ultima Online Homes For Sale");
			AddHtml( 207, 121, 418, 82, @"", (bool)true, (bool)true);
			AddLabel(214, 98, 0, @"Castles");
			AddLabel(216, 205, 0, @"Town Houses");
			AddHtml( 208, 225, 417, 91, @"", (bool)true, (bool)true);
			AddLabel(216, 322, 0, @"Custom Houses");
			AddHtml( 208, 345, 417, 91, @"", (bool)true, (bool)true);
			AddLabel(215, 438, 0, @"Pre Built Homes");
			AddHtml( 207, 460, 419, 77, @"", (bool)true, (bool)true);
			

            
        }

        

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch(info.ButtonID)
            {
                				case 0:
				{

					break;
				}

            }
        }
    }
}