namespace Server.Items
{
    public class PhillipsWoodenSteed : MonsterStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public PhillipsWoodenSteed()
            : base(MonsterStatuetteType.PhillipsWoodenSteed)
        {
            LootType = LootType.Regular;
        }

        public PhillipsWoodenSteed(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
