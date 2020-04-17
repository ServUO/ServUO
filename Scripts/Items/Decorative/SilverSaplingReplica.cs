using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class SilverSaplingReplica : Item
    {
        public override int LabelNumber => ItemID == 17095 ? 1095967 : 1095968;  // silver sapling replica - potted silver sapling replica

        [Constructable]
        public SilverSaplingReplica(int id)
            : base(id)
        {
            Weight = 5.0;
        }

        public SilverSaplingReplica(Serial serial)
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

    public class SilverSaplingReplicaDeed : Item
    {
        public override int LabelNumber => 1113934;  // Silver Sapling Deed

        [Constructable]
        public SilverSaplingReplicaDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
        }

        public SilverSaplingReplicaDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private class InternalGump : Gump
        {
            private readonly Item Deed;

            public InternalGump(Item deed)
                : base(100, 180)
            {
                Deed = deed;

                AddPage(0);

                AddBackground(25, 0, 190, 200, 0xA28);
                AddLabel(70, 12, 0x3E3, "Select one:");

                AddPage(1);

                AddItem(62, 70, 0x42C7);
                AddButton(75, 50, 0x845, 0x846, 17095, GumpButtonType.Reply, 0);
                AddItem(123, 70, 0x42C8);
                AddButton(135, 50, 0x845, 0x846, 17096, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (Deed == null || Deed.Deleted)
                    return;

                Mobile m = sender.Mobile;

                switch (info.ButtonID)
                {
                    case 0: break;
                    default:
                        {
                            m.AddToBackpack(new SilverSaplingReplica(info.ButtonID));
                            Deed.Delete();

                            break;
                        }
                }
            }
        }
    }
}
