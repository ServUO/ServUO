namespace Server.Items
{
    public class SeahorseStatuette : MonsterStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SeahorseStatuette()
            : base(MonsterStatuetteType.Seahorse)
        {
            LootType = LootType.Regular;

            Hue = Utility.RandomList(0, 0x482, 0x489, 0x495, 0x4F2);
        }

        public SeahorseStatuette(Serial serial)
            : base(serial)
        {
        }

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