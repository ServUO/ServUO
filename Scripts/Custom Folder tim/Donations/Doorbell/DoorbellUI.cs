using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;

namespace Server.Gumps
{
	public class DoorBellUI : Gump
	{
		private DoorBell Bell;
	    private List<MusicName> MusicNames;
	    private List<int> MusicLabels; 

        public DoorBellUI(DoorBell bell, List<MusicName> musicNames, List<int> musicLabels)
			: base( 60, 36 )
        {
            Bell = bell;
            MusicNames = musicNames;
            MusicLabels = musicLabels;

			AddPage( 0 );

			AddBackground( 0, 0, 273, 324, 0x13BE );
			AddImageTiled( 10, 10, 253, 20, 0xA40 );
			AddImageTiled( 10, 40, 253, 244, 0xA40 );
			AddImageTiled( 10, 294, 253, 20, 0xA40 );
			AddAlphaRegion( 10, 10, 253, 304 );
			AddButton( 10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 296, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL
			AddHtmlLocalized( 14, 12, 273, 20, 1075130, 0x7FFF, false, false ); // Choose a track to play

			int page = 1;
			int i = 0, y = 49;

			AddPage( page );

            if (musicLabels == null)
                return;

            foreach (var music in musicLabels)
            {
                if (i > 0 && i % 10 == 0)
                {
                    AddButton(228, 294, 0xFA5, 0xFA6, 0, GumpButtonType.Page, page + 1);

                    AddPage(page + 1);
                    y = 49;

                    AddButton(193, 294, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page);

                    page++;
                }

                AddButton(19, y, 0x845, 0x846, i+1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, y - 2, 213, 20, music, 0x7FFF, false, false);

                i++;
                y += 24;
            }

            if ( i % 10 == 0 )
			{
				AddButton( 228, 294, 0xFA5, 0xFA6, 0, GumpButtonType.Page, page + 1 );

				AddPage( page + 1 );
				y = 49;

				AddButton( 193, 294, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page );
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
            if (Bell == null || Bell.Deleted)
                return;

            Mobile m = sender.Mobile;
		    if (info.ButtonID < 56 && info.ButtonID > 0)
		    {
                m.Send(StopMusic.Instance);
                m.Region.Music = MusicNames[info.ButtonID-1];
                m.Send(new PlayMusic(m.Region.Music));
                m.UpdateRegion();
		    }
		}
	}
}