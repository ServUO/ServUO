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
                Duration = TimeSpan.FromMinutes(25);
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle150;
            Weight = 1.0;
        }

        public CandleShort(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID => 0x142C;
        public override int UnlitItemID => 0x142F;
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