using System;

namespace Server.Items
{
    public class MiniatureMushroom : Food
    {
        [Constructable]
        public MiniatureMushroom()
            : base(0xD16)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public MiniatureMushroom(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073138;
            }
        }// Miniature mushroom
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