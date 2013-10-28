using System;

namespace Server.Items
{
    public class BedlamKey : MasterKey
    {
        public BedlamKey()
            : base(0xFF3)
        {
        }

        public BedlamKey(Serial serial)
            : base(serial)
        {
        }

        public override int Lifespan
        {
            get
            {
                return 600;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool CanOfferConfirmation(Mobile from)
        {
            if (from.Region != null && from.Region.IsPartOf("Bedlam"))
                return base.CanOfferConfirmation(from);
				
            return false;
        }
    }
}