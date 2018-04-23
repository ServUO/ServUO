using System;

namespace Server.Items
{
    public class XmasGarland2 : BaseLight
    {
        [Constructable]
        public XmasGarland2()
            : base(40959)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 4.0;
        }

        public XmasGarland2(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 40960;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 40959;
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