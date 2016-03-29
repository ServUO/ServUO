using System;

namespace Server.Items
{
    public class PlayingCards : Item
    {
        [Constructable]
        public PlayingCards()
            : base(0xFA3)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public PlayingCards(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}