namespace Server.Items
{
    public class MaddeningHorrorCostume : BaseCostume
    {
        public override string CreatureName => "maddening horror";

        [Constructable]
        public MaddeningHorrorCostume() : base()
        {
            CostumeBody = 721;
        }

        public override int LabelNumber => 1114233;// maddening horror costume

        public MaddeningHorrorCostume(Serial serial) : base(serial)
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
