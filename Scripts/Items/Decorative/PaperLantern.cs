using System;

namespace Server.Items
{
    [Flipable]
    public class PaperLantern : BaseLight
    {
        [Constructable]
        public PaperLantern()
            : base(0x24BE)
        {
            Movable = true;
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle150;
            Weight = 3.0;
        }

        public PaperLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID => 0x24BD;
        public override int UnlitItemID => 0x24BE;
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