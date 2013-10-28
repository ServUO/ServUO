using System;

namespace Server.Items
{
    public class IngotStone : Item
    {
        [Constructable]
        public IngotStone()
            : base(0xED4)
        {
            this.Movable = false;
            this.Hue = 0x480;
        }

        public IngotStone(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "an Ingot stone";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            BagOfingots ingotBag = new BagOfingots(5000);

            if (!from.AddToBackpack(ingotBag))
                ingotBag.Delete();
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