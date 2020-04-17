namespace Server.Items
{
    public class DarkSapphireBracelet : GoldBracelet
    {
        [Constructable]
        public DarkSapphireBracelet()
            : base()
        {
            Weight = 1.0;

            BaseRunicTool.ApplyAttributesTo(this, true, 0, Utility.RandomMinMax(1, 4), 0, 100);

            if (Utility.Random(100) < 10)
                Attributes.RegenMana += 2;
            else
                Resistances.Cold += 10;
        }

        public DarkSapphireBracelet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073455;// dark sapphire bracelet
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}