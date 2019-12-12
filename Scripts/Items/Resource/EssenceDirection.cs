using System;

namespace Server.Items
{
    public class EssenceDirection : Item, ICommodity
    {
        [Constructable]
        public EssenceDirection()
            : this(1)
        {
        }

        [Constructable]
        public EssenceDirection(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
			Hue = 1156;
        }

        public EssenceDirection(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113328;
            }
        }// essence of direction
		TextDefinition ICommodity.Description
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
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
    }
}
