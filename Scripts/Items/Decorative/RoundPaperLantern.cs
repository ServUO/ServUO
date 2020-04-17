using System;

namespace Server.Items
{
    [Flipable]
    public class RoundPaperLantern : BaseLight
    {
        [Constructable]
        public RoundPaperLantern()
            : base(0x24CA)
        {
            Movable = true;
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle150;
            Weight = 3.0;
        }

        public RoundPaperLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID => 0x24C9;
        public override int UnlitItemID => 0x24CA;
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