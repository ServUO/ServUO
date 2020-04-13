using System;
using Server;

namespace Server.Items
{
    public class RareSerpentEgg : PeerlessKey
    {
        public override int LabelNumber => 1112575; // a rare serpent egg

        [Constructable]
        public RareSerpentEgg()
            : base(0x41BF)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = Utility.RandomList(0x21, 0x4AC, 0x41C);
        }

        public override int Lifespan => 43200;

        public RareSerpentEgg(Serial serial)
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
            /*int version = */
            reader.ReadInt();
        }
    }
}