using System;

namespace Server.Items
{
    public class TailorBag : Bag
    {
        [Constructable]
        public TailorBag()
            : this(1)
        {
            this.Movable = true;
            this.Hue = 0x315;
        }

        [Constructable]
        public TailorBag(int amount)
        {
            this.DropItem(new SewingKit(5));
            this.DropItem(new Scissors());
            this.DropItem(new Hides(500));
            this.DropItem(new BoltOfCloth(20));
            this.DropItem(new DyeTub());
            this.DropItem(new DyeTub());
            this.DropItem(new BlackDyeTub());
            this.DropItem(new Dyes());
        }

        public TailorBag(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "a Tailoring Kit";
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