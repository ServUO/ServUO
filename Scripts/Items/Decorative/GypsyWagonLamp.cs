using System;

namespace Server.Items
{
    public class GypsyWagonLamp : BaseLight, IFlipable
    {
        public override int LabelNumber => 1124268;  // Gypsy Wagon Lamp

        public override int LitItemID => ItemID == 0x9D36 ? 0x9D34 : 0x9D35;
        public override int UnlitItemID => ItemID == 0x9D34 ? 0x9D36 : 0x9D37;

        public int NorthID => Burning ? 0x9D34 : 0x9D36;
        public int WestID => Burning ? 0x9D35 : 0x9D37;

        [Constructable]
        public GypsyWagonLamp()
            : base(0x9D36)
        {
            Duration = Burnout ? TimeSpan.FromMinutes(60) : TimeSpan.Zero;
            Burning = false;
            Light = LightType.Circle225;
            Weight = 1.0;
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == NorthID)
                ItemID = WestID;
            else if (ItemID == WestID)
                ItemID = NorthID;
        }

        public GypsyWagonLamp(Serial serial)
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
