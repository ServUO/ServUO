using System;

namespace Server.Items
{
    public class ScouringToxin : Item
    {
        [Constructable]
        public ScouringToxin()
            : this(1)
        {
        }

        [Constructable]
        public ScouringToxin(int amount)
            : base(0x4005)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public ScouringToxin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112292;
            }
        }// scouring toxin
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