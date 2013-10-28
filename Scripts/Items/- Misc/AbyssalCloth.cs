using System;

namespace Server.Items
{
    public class AbyssalCloth : Item
    {
        [Constructable]
        public AbyssalCloth()
            : this(1)
        {
        }

        [Constructable]
        public AbyssalCloth(int amount)
            : base(0x3183)
        {
            this.Stackable = true;
            this.Amount = amount;			
			this.Hue = 2075;
        }

        public AbyssalCloth(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113350;
            }
        }// abyssal cloth
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