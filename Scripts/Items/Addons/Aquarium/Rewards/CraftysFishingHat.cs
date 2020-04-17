namespace Server.Items
{
    public class CraftysFishingHat : BaseHat
    {
        [Constructable]
        public CraftysFishingHat()
            : base(0x1713)
        {
        }

        public CraftysFishingHat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074572;// Crafty's Fishing Hat
        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1073634); // An aquarium decoration
        }

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