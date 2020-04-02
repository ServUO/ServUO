using System;

namespace Server.Items
{
    public class PhillipsWoodenSteed : MonsterStatuette
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public PhillipsWoodenSteed()
            : base(MonsterStatuetteType.PhillipsWoodenSteed)
        {
            this.LootType = LootType.Regular;
        }

        public PhillipsWoodenSteed(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties { get { return true; } }

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
