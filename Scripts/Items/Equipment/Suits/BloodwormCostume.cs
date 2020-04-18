namespace Server.Items
{
    public class BloodwormCostume : BaseCostume
    {
        public override string CreatureName => "bloodworm";

        [Constructable]
        public BloodwormCostume() : base()
        {
            CostumeBody = 287;
        }

        public override int LabelNumber => 1114006;// bloodworm halloween costume

        public BloodwormCostume(Serial serial) : base(serial)
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
