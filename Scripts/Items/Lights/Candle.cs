using System;

namespace Server.Items
{
    public class Candle : BaseEquipableLight
    {
        [Constructable]
        public Candle()
            : base(0xA28)
        {
            if (Burnout)
                this.Duration = TimeSpan.FromMinutes(20);
            else
                this.Duration = TimeSpan.Zero;

            this.Burning = false;
            this.Light = LightType.Circle150;
            this.Weight = 1.0;
        }

        public Candle(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0xA0F;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0xA28;
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
}