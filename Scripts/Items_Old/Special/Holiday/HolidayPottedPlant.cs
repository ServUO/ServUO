using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class HolidayPottedPlant : Item
    {
        [Constructable]
        public HolidayPottedPlant()
            : this(Utility.RandomMinMax(0x11C8, 0x11CC))
        {
        }

        [Constructable]
        public HolidayPottedPlant(int itemID)
            : base(itemID)
        {
        }

        public HolidayPottedPlant(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class PottedPlantDeed : Item
    {
        [Constructable]
        public PottedPlantDeed()
            : base(0x14F0)
        {
            this.LootType = LootType.Blessed;
        }

        public PottedPlantDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041114;
            }
        }// A deed for a potted plant.
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
            {
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private class InternalGump : Gump
        {
            private readonly PottedPlantDeed m_Deed;
            public InternalGump(PottedPlantDeed deed)
                : base(100, 200)
            {
                this.m_Deed = deed;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);
                this.AddBackground(0, 0, 360, 195, 0xA28);

                this.AddPage(1);
                this.AddLabel(45, 15, 0, "Choose a Potted Plant:");

                this.AddItem(45, 75, 0x11C8);
                this.AddButton(55, 50, 0x845, 0x846, 1, GumpButtonType.Reply, 0);

                this.AddItem(100, 75, 0x11C9);
                this.AddButton(115, 50, 0x845, 0x846, 2, GumpButtonType.Reply, 0);

                this.AddItem(160, 75, 0x11CA);
                this.AddButton(175, 50, 0x845, 0x846, 3, GumpButtonType.Reply, 0);

                this.AddItem(225, 75, 0x11CB);
                this.AddButton(235, 50, 0x845, 0x846, 4, GumpButtonType.Reply, 0);

                this.AddItem(280, 75, 0x11CC);
                this.AddButton(295, 50, 0x845, 0x846, 5, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Deed == null || this.m_Deed.Deleted)
                    return;

                Mobile from = sender.Mobile;

                if (!this.m_Deed.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it
                    return;
                }

                int index = info.ButtonID - 1;

                if (index >= 0 && index <= 4)
                {
                    HolidayPottedPlant plant = new HolidayPottedPlant(0x11C8 + index);

                    if (!from.PlaceInBackpack(plant))
                    {
                        plant.Delete();
                        from.SendLocalizedMessage(1078837); // Your backpack is full! Please make room and try again.
                    }
                    else
                    {
                        this.m_Deed.Delete();
                    }
                }
            }
        }
    }
}