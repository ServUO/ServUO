namespace Server.Items
{
    public class WardensArmorOfAugmentation : GargishLeatherWingArmor
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115515;  // Warden's Armor Of Augmentation

        [Constructable]
        public WardensArmorOfAugmentation()
        {
            Hue = 0x9C2;
            AbsorptionAttributes.EaterKinetic = 5;
            Attributes.SpellDamage = 3;
            Attributes.LowerManaCost = 1;
            Attributes.WeaponSpeed = 5;
        }

        public WardensArmorOfAugmentation(Serial serial)
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