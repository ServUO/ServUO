namespace Server.Items
{
    public class SkitteringHopperCostume : BaseCostume
    {
        public override string CreatureName => "skittering hopper";

        [Constructable]
        public SkitteringHopperCostume() : base()
        {
            CostumeBody = 302;
        }

        public override int LabelNumber => 1114240;// skittering hopper costume

        public SkitteringHopperCostume(Serial serial) : base(serial)
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
