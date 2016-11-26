using System;
using Server;

namespace Server.Items
{
    public class WardedDemonboneBracers : BoneArms
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115775; } } // Warded Demonbone Bracers

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int PhysicalResistance { get { return 6; } }
        public override int FireResistance { get { return 8; } }
        public override int ColdResistance { get { return 4; } }
        public override int PoisonResistance { get { return 5; } }
        public override int EnergyResistance { get { return 5; } }

        [Constructable]
        public WardedDemonboneBracers()
        {
            Hue = 0x2E2;

            AbsorptionAttributes.CastingFocus = 2;
            Attributes.RegenMana = 1;
            Attributes.LowerManaCost = 6;
            Attributes.LowerRegCost = 12;
            ArmorAttributes.MageArmor = 1;
        }

        public WardedDemonboneBracers(Serial serial)
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