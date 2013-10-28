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
                this.Duration = TimeSpan.FromMinutes(20);
            else
                this.Duration = TimeSpan.Zero;

            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 2.0;
        }

        public Lantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                if (this.ItemID == 0xA15 || this.ItemID == 0xA17)
                    return this.ItemID;

                return 0xA22;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                if (this.ItemID == 0xA18)
                    return this.ItemID;

                return 0xA25;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LanternOfSouls : Lantern
    {
        [Constructable]
        public LanternOfSouls()
        {
            this.Hue = 0x482;
        }

        public LanternOfSouls(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061618;
            }
        }// Lantern of Souls
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}