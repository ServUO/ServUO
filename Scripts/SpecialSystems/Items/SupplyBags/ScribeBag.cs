using System;

namespace Server.Items
{
    public class ScribeBag : Bag
    {
        [Constructable]
        public ScribeBag()
            : this(1)
        {
            this.Movable = true;
            this.Hue = 0x105;
        }

        [Constructable]
        public ScribeBag(int amount)
        {
            this.DropItem(new BagOfReagents(5000));
            this.DropItem(new BlankScroll(500));
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