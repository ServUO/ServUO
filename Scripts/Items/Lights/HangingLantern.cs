using System;

namespace Server.Items
{
    public class HangingLantern : BaseLight
    {
        [Constructable]
        public HangingLantern()
            : base(0xA1D)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 40.0;
        }

        public HangingLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0xA1A;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0xA1D;
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