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
                this.Duration = TimeSpan.FromMinutes(25);
            else
                this.Duration = TimeSpan.Zero;

            this.Burning = false;
            this.Light = LightType.Circle150;
            this.Weight = 2.0;
        }

        public CandleLarge(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0xB1A;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0xA26;
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