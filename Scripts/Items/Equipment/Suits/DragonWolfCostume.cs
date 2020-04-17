namespace Server.Items
{
    public class DragonWolfCostume : BaseCostume
    {
        public override string CreatureName => "dragon wolf";

        [Constructable]
        public DragonWolfCostume() : base()
        {
            CostumeBody = 719;
        }

        public override string DefaultName => "a dragon wolf costume";

        public DragonWolfCostume(Serial serial) : base(serial)
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
