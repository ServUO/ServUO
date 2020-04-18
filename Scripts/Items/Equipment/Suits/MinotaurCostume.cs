namespace Server.Items
{
    public class MinotaurCostume : BaseCostume
    {
        public override string CreatureName => "minotaur";

        [Constructable]
        public MinotaurCostume() : base()
        {
            CostumeBody = 263;
        }

        public override int LabelNumber => 1114237;// minotaur costume

        public MinotaurCostume(Serial serial) : base(serial)
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
