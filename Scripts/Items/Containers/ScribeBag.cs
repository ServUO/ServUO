using System;

namespace Server.Items
{
    public class ScribeBag : Bag
    {
        [Constructable]
        public ScribeBag()
        {
            Hue = 0x105;

            DropItem(new BagOfReagents(5000));
            DropItem(new BlankScroll(500));
        }

        public ScribeBag(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "a Scribe Kit";
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
