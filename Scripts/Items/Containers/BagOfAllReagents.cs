using System;

namespace Server.Items
{
    public class BagOfAllReagents : Bag
    {
        [Constructable]
        public BagOfAllReagents()
            : this(50)
        {
        }

        [Constructable]
        public BagOfAllReagents(int amount)
        {
            this.DropItem(new BlackPearl(amount));
            this.DropItem(new Bloodmoss(amount));
            this.DropItem(new Garlic(amount));
            this.DropItem(new Ginseng(amount));
            this.DropItem(new MandrakeRoot(amount));
            this.DropItem(new Nightshade(amount));
            this.DropItem(new SulfurousAsh(amount));
            this.DropItem(new SpidersSilk(amount));
            this.DropItem(new BatWing(amount));
            this.DropItem(new GraveDust(amount));
            this.DropItem(new DaemonBlood(amount));
            this.DropItem(new NoxCrystal(amount));
            this.DropItem(new PigIron(amount));
        }

        public BagOfAllReagents(Serial serial)
            : base(serial)
        {
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