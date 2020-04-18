using System;

namespace Server.Items
{
    public class HangingLantern : BaseLight
    {
        [Constructable]
        public HangingLantern()
            : base(0xA1D)
        {
            Movable = false;
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle300;
            Weight = 40.0;
        }

        public HangingLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID => 0xA1A;
        public override int UnlitItemID => 0xA1D;
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