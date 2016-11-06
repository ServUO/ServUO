using System;

namespace Server.Items
{
    public class SherryTheMouseStatue : Item
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SherryTheMouseStatue()
            : base(0x20D0)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public SherryTheMouseStatue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080171;
            }
        }// Sherry the Mouse Statue
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