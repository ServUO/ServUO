using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Engines.Plants
{
    public class FertileDirtGump : Gump
    {
        private Seed m_Seed;
        private object m_AttachTo;

        private const int LabelColor = 0x7FFF;
        private const int FontColor = 0xFFFFFF;

        public FertileDirtGump(Seed seed, int amount, object attachTo)
            : base(50, 50)
        {
            m_Seed = seed;
            m_AttachTo = attachTo;

            AddBackground(0, 0, 300, 210, 9200);
            AddImageTiled(5, 5, 290, 30, 2624);
            AddImageTiled(5, 40, 290, 100, 2624);
            AddImageTiled(5, 145, 150, 60, 2624);
            AddImageTiled(160, 145, 135, 60, 2624);

            AddHtmlLocalized(90, 10, 150, 16, 1150359, LabelColor, false, false); // Raised Garden Bed
            AddHtmlLocalized(10, 45, 280, 90, 1150363, LabelColor, false, false);

            AddHtmlLocalized(10, 150, 80, 16, 1150361, LabelColor, false, false); // Needed:
            AddHtmlLocalized(10, 180, 80, 16, 1150360, LabelColor, false, false); // You Have:

            AddHtml(80, 150, 60, 16, String.Format("<BASEFONT COLOR=#{0:X6}>20</BASEFONT>", FontColor), false, false);
            AddHtml(80, 180, 60, 16, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", FontColor, amount.ToString()), false, false);

            AddButton(165, 150, 4023, 4025, 1, GumpButtonType.Reply, 0);
            AddButton(165, 180, 4017, 4019, 2, GumpButtonType.Reply, 0);

            AddHtml(205, 150, 100, 16, String.Format("<BASEFONT COLOR=#{0:X6}>Use</BASEFONT>", FontColor), false, false);
            AddHtmlLocalized(205, 180, 100, 16, 1150364, LabelColor, false, false); // Not Use
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            Mobile from = state.Mobile;
            bool fertile = info.ButtonID == 1;

            if (fertile && (from.Backpack == null || !from.Backpack.ConsumeTotal(typeof(FertileDirt), 20, false)))
            {
                from.SendLocalizedMessage(1150366); // You don't have enough fertile dirt in the top level of your backpack.
                return;
            }

            GardenAddonComponent comp = m_AttachTo as GardenAddonComponent;
            LandTarget lt = m_AttachTo as LandTarget;

            if (comp != null && m_Seed != null)
            {
                RaisedGardenPlantItem dirt = new RaisedGardenPlantItem(fertile);
                dirt.MoveToWorld(new Point3D(comp.X, comp.Y, comp.Z + 5), comp.Map);
                dirt.Component = comp;
                comp.Plant = dirt;
                dirt.PlantSeed(from, m_Seed);
            }
            else if (lt != null)
            {
                MaginciaPlantItem dirt = new MaginciaPlantItem(fertile);
                dirt.MoveToWorld(((LandTarget)m_AttachTo).Location, from.Map);
                dirt.Owner = from;
                dirt.StartTimer();
            }
        }
    }
}
