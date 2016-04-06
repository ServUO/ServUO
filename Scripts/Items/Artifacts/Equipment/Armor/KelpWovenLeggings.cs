using Server;
using System;

namespace Server.Items
{
    public class KelpWovenLeggings : LeatherLegs
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1149960; } }

        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 13; } }
        public override int BaseColdResistance { get { return 12; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 14; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public KelpWovenLeggings()
        {
            Hue = 1155;

            AbsorptionAttributes.CastingFocus = 4;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 8;
            Attributes.LowerRegCost = 15;
        }

        public KelpWovenLeggings(Serial serial)
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