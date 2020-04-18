namespace Server.Items
{
    public class SkeletonCostume : BaseCostume
    {
        public override string CreatureName => "skeleton";

        [Constructable]
        public SkeletonCostume() : base()
        {
            CostumeBody = 50;
        }

        public override int LabelNumber => 1113996;// skeleton halloween costume

        public SkeletonCostume(Serial serial) : base(serial)
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
