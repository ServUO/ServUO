using System;

namespace Server.Items
{
    public class SmithStone : Item
    {
        [Constructable]
        public SmithStone()
            : base(0xED4)
        {
            this.Movable = false;
            this.Hue = 0x476;
        }

        public SmithStone(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "a Blacksmith Supply Stone";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            SmithBag SmithBag = new SmithBag(5000);

            if (!from.AddToBackpack(SmithBag))
                SmithBag.Delete();
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