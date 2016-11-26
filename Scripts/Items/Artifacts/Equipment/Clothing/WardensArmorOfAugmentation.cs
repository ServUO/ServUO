using System;
using Server;

namespace Server.Items
{
    public class WardensArmorOfAugmentation : GargishLeatherWingArmor
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115515; } } // Warden's Armor Of Augmentation

        [Constructable]
        public WardensArmorOfAugmentation()
        {
            this.Hue = 0x9C2;

            this.AbsorptionAttributes.EaterKinetic = 5;
            this.Attributes.SpellDamage = 3;
            this.Attributes.LowerManaCost = 1;
            this.Attributes.WeaponSpeed = 5;
        }

        public WardensArmorOfAugmentation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}