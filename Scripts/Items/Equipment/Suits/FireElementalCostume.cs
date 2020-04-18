namespace Server.Items
{
    public class FireElementalCostume : BaseCostume
    {
        public override string CreatureName => "fire elemental";

        [Constructable]
        public FireElementalCostume() : base()
        {
            CostumeBody = 15;
        }

        public override int LabelNumber => 1114224;// fire elemental costume

        public FireElementalCostume(Serial serial) : base(serial)
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
