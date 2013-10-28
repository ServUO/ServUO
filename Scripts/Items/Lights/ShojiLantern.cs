using System;

namespace Server.Items
{
    [Flipable]
    public class ShojiLantern : BaseLight
    {
        [Constructable]
        public ShojiLantern()
            : base(0x24BC)
        {
            this.Movable = true;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle150;
            this.Weight = 3.0;
        }

        public ShojiLantern(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0x24BB;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0x24BC;
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