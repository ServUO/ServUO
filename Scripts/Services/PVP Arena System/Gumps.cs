using Server;
using System;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.ArenaSystem
{
    public class ArenaStoneGump : BaseGump
    {
        public ArenaStoneGump(PlayerMobile pm, PVPArena arena)
            : base(pm)
        {
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 300, 295, 9200);

            AddImageTiled(8, 5, 284, 30, 2624);
            AddAlphaRegion(8, 5, 284, 30);

            AddImageTiled(8, 40, 284, 255, 2624);
            AddAlphaRegion(8, 40, 284, 255);

            AddImageTiled(8, 265, 284, 25, 2624);
            AddAlphaRegion(8, 265, 284, 25);

            AddHtmlLocalized(0, 15, 300, 20, CenterLoc, "#1115619", 0xFFFF, false, false); // Arena Menu - Main
            AddHtmlLocalized(45, 105, 200, 20, 1115621, 0xFFFF, false, false); // Host a duel
            AddHtmlLocalized(45, 130, 200, 20, 1115622, 0xFFFF, false, false); // Join a duel
            AddHtmlLocalized(45, 155, 200, 20, 1115925, 0xFFFF, false, false); // See booked duels
            AddHtmlLocalized(45, 180, 200, 20, 1115623, 0xFFFF, false, false); // Check your stats
            AddHtmlLocalized(45, 205, 200, 20, 1150128, 0xFFFF, false, false); // Check arena rankings
            AddHtmlLocalized(45, 230, 200, 20, ignore ? 1116155 : 1116156, 0xFFFF, false, false); // Ignore duel invite (ON) / Ignore duel invite (OFF)

            AddButton(10, 265, 4023, 4025, 1, GumpButtonType.Reply, 0);
            AddButton(10, 265, 4023, 4025, 2, GumpButtonType.Reply, 0);
            AddButton(10, 265, 4023, 4025, 3, GumpButtonType.Reply, 0);
            AddButton(10, 265, 4023, 4025, 4, GumpButtonType.Reply, 0);
            AddButton(10, 265, 4023, 4025, 5, GumpButtonType.Reply, 0);
            AddButton(10, 265, 4023, 4025, 6, GumpButtonType.Reply, 0);

            AddHtmlLocalized(42, 265, 150, 20, 1150300, 0xFFFF, false, false); // CANCEL
            AddButton(8, 265, 4017, 4019, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResonse(RelayInfo info)
        {

        }
    }
}