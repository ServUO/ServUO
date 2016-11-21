using System;
using Server;

namespace Server.Items
{
    public class RangersCloakOfAugmentation : Cloak
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115514; } } // Ranger's Cloak Of Augmentation

        [Constructable]
        public RangersCloakOfAugmentation()
        {
            this.Hue = 0x54A;

            this.SAAbsorptionAttributes.EaterKinetic = 5;
            this.Attributes.SpellDamage = 3;
            this.Attributes.LowerManaCost = 1;
            this.Attributes.WeaponSpeed = 5;
        }

        public RangersCloakOfAugmentation(Serial serial)
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