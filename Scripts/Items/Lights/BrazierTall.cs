using System;

namespace Server.Items
{
    public class BrazierTall : BaseLight
    {
        [Constructable]
        public BrazierTall()
            : base(0x19AA)
        {
            this.Movable = false;
            this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = true;
            this.Light = LightType.Circle300;
            this.Weight = 25.0;
        }

        public BrazierTall(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0x19AA;
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