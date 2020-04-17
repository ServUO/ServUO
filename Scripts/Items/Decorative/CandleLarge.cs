using System;

namespace Server.Items
{
    public class CandleLarge : BaseLight
    {
        [Constructable]
        public CandleLarge()
            : base(0xA26)
        {
            if (Burnout)
                Duration = TimeSpan.FromMinutes(25);
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle150;
            Weight = 2.0;
        }

        public CandleLarge(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID => 0xB1A;
        public override int UnlitItemID => 0xA26;
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