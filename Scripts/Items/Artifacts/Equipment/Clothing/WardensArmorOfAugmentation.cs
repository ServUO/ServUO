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
            Hue = 0x9C2;
            SkillBonuses.SetValues(0, RangersCloakOfAugmentation.GetRandomRangerSkill(), 20.0);
            AbsorptionAttributes.EaterKinetic = 10;
            Attributes.SpellDamage = 5;
            Attributes.LowerManaCost = 8;
            Attributes.WeaponSpeed = 5;
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