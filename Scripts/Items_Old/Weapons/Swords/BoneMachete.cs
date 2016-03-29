using System;

namespace Server.Items
{
    public class BoneMachete : ElvenMachete
    {
        [Constructable]
        public BoneMachete()
        {
            // TODO attributes
        }

        public BoneMachete(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1020526;
            }
        }// bone machete
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