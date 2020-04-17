namespace Server.Items
{
    public class GoreFiendCostume : BaseCostume
    {
        public override string CreatureName => "gore fiend";

        [Constructable]
        public GoreFiendCostume() : base()
        {
            CostumeBody = 305;
        }

        public override int LabelNumber => 1114227;// gore fiend costume

        public GoreFiendCostume(Serial serial) : base(serial)
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
