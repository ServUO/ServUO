using System;

namespace Server.Items
{
    public class EssenceAchievement : Item, ICommodity
    {
        [Constructable]
        public EssenceAchievement()
            : this(1)
        {
        }

        [Constructable]
        public EssenceAchievement(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
			Hue = 1724;
        }

        public EssenceAchievement(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113325;
            }
        }// essence of achievement
		int ICommodity.DescriptionNumber
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
