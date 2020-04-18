using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class HolidayTreeDeed : Item
    {
        [Constructable]
        public HolidayTreeDeed()
            : base(0x14F0)
        {
            Hue = 0x488;
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public HolidayTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041116;// a deed for a holiday tree
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            LootType = LootType.Blessed;
        }

        public bool ValidatePlacement(Mobile from, Point3D loc)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return false;
            }

            if (DateTime.UtcNow.Month != 12)
            {
                from.SendLocalizedMessage(1005700); // You will have to wait till next December to put your tree back up for display.
                return false;
            }

            Map map = from.Map;

            if (map == null)
                return false;

            BaseHouse house = BaseHouse.FindHouseAt(loc, map, 20);

            if (house == null || !house.IsFriend(from))
            {
                from.SendLocalizedMessage(1005701); // The holiday tree can only be placed in your house.
                return false;
            }

            if (!map.CanFit(loc, 20))
            {
                from.SendLocalizedMessage(500269); // You cannot build that there.
                return false;
            }

            return true;
        }

        public void BeginPlace(Mobile from, HolidayTreeType type)
        {
            from.BeginTarget(-1, true, TargetFlags.None, new TargetStateCallback(Placement_OnTarget), type);
        }

        public void Placement_OnTarget(Mobile from, object targeted, object state)
        {
            IPoint3D p = targeted as IPoint3D;

            if (p == null)
                return;

            Point3D loc = new Point3D(p);

            if (p is StaticTarget)
                loc.Z -= TileData.ItemTable[((StaticTarget)p).ItemID].CalcHeight;	/* NOTE: OSI does not properly normalize Z positioning here.
            * A side affect is that you can only place on floors (due to the CanFit call).
            * That functionality may be desired. And so, it's included in this script.
            */

            if (ValidatePlacement(from, loc))
                EndPlace(from, (HolidayTreeType)state, loc);
        }

        public void EndPlace(Mobile from, HolidayTreeType type, Point3D loc)
        {
            Delete();
            HolidayTree tree = new HolidayTree(from, type, loc);
            BaseHouse house = BaseHouse.FindHouseAt(tree);
            if (house != null)
                house.Addons[tree] = from;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseGump(typeof(HolidayTreeChoiceGump));
            from.SendGump(new HolidayTreeChoiceGump(from, this));
        }
    }

    public class HolidayTreeChoiceGump : Gump
    {
        private readonly Mobile m_From;
        private readonly HolidayTreeDeed m_Deed;
        public HolidayTreeChoiceGump(Mobile from, HolidayTreeDeed deed)
            : base(200, 200)
        {
            m_From = from;
            m_Deed = deed;

            AddPage(0);

            AddBackground(0, 0, 220, 120, 5054);
            AddBackground(10, 10, 200, 100, 3000);

            AddButton(20, 35, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 35, 145, 25, 1018322, false, false); // Classic

            AddButton(20, 65, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 65, 145, 25, 1018321, false, false); // Modern
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Deed.Deleted)
                return;

            switch (info.ButtonID)
            {
                case 1:
                    {
                        m_Deed.BeginPlace(m_From, HolidayTreeType.Classic);
                        break;
                    }
                case 2:
                    {
                        m_Deed.BeginPlace(m_From, HolidayTreeType.Modern);
                        break;
                    }
            }
        }
    }
}