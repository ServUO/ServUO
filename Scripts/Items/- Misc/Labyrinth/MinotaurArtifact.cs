using System;

namespace Server.Items
{
    public class MinotaurArtifact : Item
    {
        [Constructable]
        public MinotaurArtifact()
            : base(Utility.RandomList(0xB46, 0xB48, 0x9ED))
        {
            if (this.ItemID == 0x9ED)
                this.Weight = 30;

            this.LootType = LootType.Blessed;
            this.Hue = 0x100;
        }

        public MinotaurArtifact(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074826;
            }
        }// Minotaur Artifact
        public override double DefaultWeight
        {
            get
            {
                return 5.0;
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