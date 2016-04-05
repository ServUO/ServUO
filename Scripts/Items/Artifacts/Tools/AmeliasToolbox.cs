using System;

namespace Server.Items
{
    public class AmeliasToolbox : TinkerTools
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AmeliasToolbox()
            : base(500)
        {
            this.LootType = LootType.Blessed;
            this.Hue = 1895; // TODO check
        }

        public AmeliasToolbox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1077749;
            }
        }// Amelias Toolbox
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}