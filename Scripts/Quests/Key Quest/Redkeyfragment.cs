using System;

namespace Server.Items
{
    public class Redkeyfragment : Item
    {
        [Constructable]
        public Redkeyfragment()
            : base(0x1012)
        {
            this.Movable = false;
            this.Hue = 0x8F;
        }

        public Redkeyfragment(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1111647;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("You make a copy of the key in your pack");
			
            RedKey1 redkey = new RedKey1();
            if (!from.AddToBackpack(redkey))
                redkey.Delete();
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