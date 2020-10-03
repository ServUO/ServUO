namespace Server.Items
{
    public class HelmOfVengence : NorseHelm
    {
        public override int LabelNumber => 1116621;

        public override int BasePhysicalResistance => 11;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 14;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 8;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public HelmOfVengence()
        {
            Hue = 2012;
            Attributes.RegenMana = 3;
            Attributes.ReflectPhysical = 30;
            Attributes.AttackChance = 7;
            Attributes.WeaponDamage = 10;
            Attributes.LowerManaCost = 8;
        }

        public HelmOfVengence(Serial serial)
            : base(serial)
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