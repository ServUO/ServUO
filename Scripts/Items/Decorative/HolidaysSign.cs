using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class HolidaysSign : Item, IFlipable
    {
        public override int LabelNumber => 1024759;  // sign

        public int GumpID { get; set; }

        [Constructable]
        public HolidaysSign()
            : base(0xA130)
        {
            GumpID = 1673;
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == 0xA130 || ItemID == 0xA131)
                ItemID = ItemID + 4;
            else
                ItemID = ItemID - 4;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(GetWorldLocation(), 3))
            {
                m.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else
            {
                Gump g = new Gump(100, 100);
                g.AddImage(0, 0, GumpID);

                m.SendGump(g);
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from))
            {
                list.Add(new ChangeSignText(this, from));
                list.Add(new ToggleOnOff(this));
            }
            else
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
        }

        private static readonly Dictionary<int, string> SingType = new Dictionary<int, string>()
        {
            {1673,"Happy Holidays"},
            {1674,"Merry Christmas"},
            {1675,"Seasons Greetings"},
            {1676,"Happy Hanukkah"},
        };

        private class ChangeSignText : ContextMenuEntry
        {
            private readonly HolidaysSign Sign;
            private readonly Mobile _From;

            public ChangeSignText(HolidaysSign sign, Mobile from)
                : base(1158829)
            {
                Sign = sign;
                _From = from;
            }

            public override void OnClick()
            {
                if (Sign.GumpID == 1676)
                    Sign.GumpID = 1673;
                else
                    Sign.GumpID++;

                _From.SendLocalizedMessage(1158830, string.Format("{0}", SingType.ToList().Find(x => x.Key == Sign.GumpID).Value)); // The sign text has been set to: ~1_TEXT~
            }
        }

        private class ToggleOnOff : ContextMenuEntry
        {
            private readonly Item Sign;

            public ToggleOnOff(Item sign)
                : base(1155742)
            {
                Sign = sign;
            }

            public override void OnClick()
            {
                int ItemID = Sign.ItemID;

                if (ItemID == 0xA130 || ItemID == 0xA134)
                    Sign.ItemID = ItemID + 1;
                else
                    Sign.ItemID = ItemID - 1;
            }
        }

        public HolidaysSign(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(GumpID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            GumpID = reader.ReadInt();
        }
    }
}
