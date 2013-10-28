using System;

namespace Server.Items
{
    public class SimpleArtifact : Artifact
    {
        private int m_ArtifactRarity = 0;
        [Constructable]
        public SimpleArtifact(int itemID)
            : base(itemID)
        {
        }

        public SimpleArtifact(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ArtifactRarity
        { 
            get
            {
                return this.m_ArtifactRarity;
            }
            set
            {
                this.m_ArtifactRarity = value;
                this.InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_ArtifactRarity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_ArtifactRarity = reader.ReadInt();
        }
    }
}