using System;

using Server.Engines.Craft;

namespace Server.Items
{
    public class SummonersKilt : GargishClothKilt, IRepairable
	{
		public Server.Engines.Craft.CraftSystem RepairSystem { get { return DefTailoring.CraftSystem; } }
		
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113540; } } // Summoner's Kilt
		
        [Constructable]
        public SummonersKilt()
        {
            Hue = 1266;			
            Attributes.BonusMana = 5;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 5;
            SAAbsorptionAttributes.CastingFocus = 2;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 10;
        }

        public SummonersKilt(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 21;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 21;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
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
