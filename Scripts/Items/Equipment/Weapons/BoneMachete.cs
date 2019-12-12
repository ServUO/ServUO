using System;

namespace Server.Items
{
    public class BoneMachete : ElvenMachete
    {
        public override int InitMinHits { get { return 1; } }
        public override int InitMaxHits { get { return 3; } }
		public override int LabelNumber { get { return 1020526; } }// bone machete

        [Constructable]
        public BoneMachete()
        {
        }

        public BoneMachete(Serial serial)
            : base(serial)
        {
        } 

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();           
        }
    }
}