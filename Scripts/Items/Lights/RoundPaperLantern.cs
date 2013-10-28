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
            this.Movable = true;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle150;
            this.Weight = 3.0;
        }

        public RoundPaperLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0x24C9;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0x24CA;
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