using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.Points
{
    public class PointExchanceStone : Item
    {
        public override int LabelNumber => 1158449;  // Cleanup Point Exchange
        public override bool ForceShowProperties => true;

        [Constructable]
        public PointExchanceStone()
            : base(0xEDD)
        {
            Hue = 1037;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!CleanUpBritanniaData.Enabled)
            {
                base.OnDoubleClick(m);
                return;
            }

            if (m.InRange(Location, 3))
            {
                if (m is PlayerMobile)
                {
                    m.CloseGump(typeof(InternalGump));
                    BaseGump.SendGump(new InternalGump((PlayerMobile)m));
                }
            }
            else
            {
                m.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        private class InternalGump : BaseGump
        {
            public CleanUpBritanniaData System => PointsSystem.CleanUpBritannia;

            public InternalGump(PlayerMobile pm)
                : base(pm, 100, 100)
            {
            }

            public override void AddGumpLayout()
            {
                int points = (int)PointsSystem.CleanUpBritannia.GetPoints(User);
                int accountPoints = (int)PointsSystem.CleanUpBritannia.GetPointsFromExchange(User);

                AddBackground(0, 0, 370, 245, 0x53);
                AddHtmlLocalized(0, 15, 370, 20, CenterLoc, "#1158449", 0x7FFF, false, false);

                AddHtmlLocalized(15, 40, 340, 20, 1158454, points.ToString("N0"), 0x7FFF, false, false); // Your Cleanup Point Balance: ~1_VALUE~
                AddHtmlLocalized(15, 70, 340, 20, 1158455, accountPoints.ToString("N0"), 0x7FFF, false, false); // Points Currently in Exchange: ~1_VALUE~

                AddButton(20, 105, 4005, 4006, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(60, 105, 100, 20, 1158466, 0x7FFF, false, false); // Deposit Points

                AddButton(20, 127, 4005, 4006, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(60, 127, 100, 20, 1158465, 0x7FFF, false, false); // Withdraw Points

                AddHtmlLocalized(15, 170, 340, 20, 1158459, points.ToString("N0"), C32216(0xb478ed), false, false); // You can currently deposit ~1_VALUE~ points.
                AddHtmlLocalized(15, 200, 340, 20, 1158458, accountPoints.ToString("N0"), C32216(0xb478ed), false, false); // You can currently withdraw ~1_VALUE~ points.
            }

            public override void OnResponse(RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case 0:
                        return;
                    case 1:
                        PointsSystem.CleanUpBritannia.AddPointsToExchange(User);
                        break;
                    case 2:
                        PointsSystem.CleanUpBritannia.RemovePointsFromExchange(User);
                        break;
                }

                Refresh();
            }
        }

        public PointExchanceStone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}