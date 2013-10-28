using System;

namespace Server.Items
{
    public class ToxicVenomSac : Item
    {
        [Constructable]
        public ToxicVenomSac()
            : this(1)
        {
        }

        [Constructable]
        public ToxicVenomSac(int amount)
            : base(0x4005)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public ToxicVenomSac(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112291;
            }
        }// toxic venom sac
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
    }
}