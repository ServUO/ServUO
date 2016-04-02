using System;

namespace Server.Items
{
    public class AdmiralsHeartyRum : BeverageBottle
    {
        [Constructable]
        public AdmiralsHeartyRum()
            : base(BeverageType.Ale)
        {
            this.Hue = 0x66C;
        }

        public AdmiralsHeartyRum(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063477;
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