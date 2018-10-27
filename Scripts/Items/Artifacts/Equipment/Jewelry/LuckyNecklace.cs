using System;

namespace Server.Items
{
    public class LuckyNecklace : BaseJewel
	{
        [Constructable]
        public LuckyNecklace()
            : base(0x1088, Layer.Neck)
        {
            Attributes.Luck = 200;
            LootType = LootType.Blessed;
        }

        public LuckyNecklace(Serial serial)
            : base(serial)
        {
        }

        public override int Hue
        {
            get
            {
                return 1150;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1075239;
            }
        }//Lucky Necklace	1075239
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt(); /* int version = reader.ReadInt(); Why? Just to have an unused var? */
        }
    }
}