using System;

namespace Server.Items
{
    public class CandleShort : BaseLight
    {
        [Constructable]
        public CandleShort()
            : base(0x142F)
        {
            if (Burnout)
                this.Duration = TimeSpan.FromMinutes(25);
            else
                this.Duration = TimeSpan.Zero;

            this.Burning = false;
            this.Light = LightType.Circle150;
            this.Weight = 1.0;
        }

        public CandleShort(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0x142C;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0x142F;
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