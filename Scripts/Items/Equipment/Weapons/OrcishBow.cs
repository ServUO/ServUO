namespace Server.Items
{
    public class OrcishBow : Bow
    {
        public override int LabelNumber => 1153778;  // an orcish bow

        [Constructable]
        public OrcishBow()
        {
            Hue = 1107;
            Attributes.WeaponDamage = 25;
            WeaponAttributes.DurabilityBonus = 70;
        }

        public OrcishBow(Serial serial)
            : base(serial)
        {
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