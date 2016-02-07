using System;

namespace Server.Items
{
    public class AlchemyStone : Item
    {
        [Constructable]
        public AlchemyStone()
            : base(0xED4)
        {
            this.Movable = false;
            this.Hue = 0x250;
        }

        public AlchemyStone(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "an Alchemist Supply Stone";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            AlchemyBag alcBag = new AlchemyBag();

            if (!from.AddToBackpack(alcBag))
                alcBag.Delete();
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