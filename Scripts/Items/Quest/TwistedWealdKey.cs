using System;

namespace Server.Items
{
    public class TwistedWealdKey : MasterKey
    {
        public TwistedWealdKey()
            : base(0xE26)
        {
            this.Weight = 1.0;
            this.Hue = 0x481;
        }

        public TwistedWealdKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075802;
            }
        }// Essence of Wind
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
            if (from.Region != null && from.Region.IsPartOf("Twisted Weald"))
                return base.CanOfferConfirmation(from);
				
            return false;
        }
    }
}