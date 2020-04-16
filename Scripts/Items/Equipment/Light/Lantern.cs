using System;

namespace Server.Items
{
    public class Lantern : BaseEquipableLight
    {
        [Constructable]
        public Lantern()
            : base(0xA25)
        {
            if (Burnout)
                Duration = TimeSpan.FromMinutes(20);
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle300;
            Weight = 2.0;
        }

        public Lantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (ItemID == 0xA15 || ItemID == 0xA17)
                    return ItemID;

                return 0xA22;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (ItemID == 0xA18)
                    return ItemID;

                return 0xA25;
            }
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

    public class LanternOfSouls : Lantern
    {
        public override int LabelNumber => 1061618; // Lantern of Souls

        [Constructable]
        public LanternOfSouls()
        {
            Hue = 0x482;
        }

        public LanternOfSouls(Serial serial)
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