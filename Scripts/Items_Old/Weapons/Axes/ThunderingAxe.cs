using System;

namespace Server.Items
{
    public class ThunderingAxe : OrnateAxe
    {
        [Constructable]
        public ThunderingAxe()
        {
            this.WeaponAttributes.HitLightning = 10;
        }

        public ThunderingAxe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073547;
            }
        }// thundering axe
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}