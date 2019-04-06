using System;

namespace Server.Items
{
    public class BasiliskHideBreastplate : DragonChest
    {
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115444; } } // Basilisk Hide Breastplate

        [Constructable]
        public BasiliskHideBreastplate() 
        {
            Resource = CraftResource.None;
            Hue = 1366;
            AbsorptionAttributes.EaterDamage = 10;
            Attributes.BonusDex = 5;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 2;
            Attributes.RegenMana = 1;
            Attributes.DefendChance = 5;
            Attributes.LowerManaCost = 5;
        }

        public BasiliskHideBreastplate(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 14;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 11;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 5;
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
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Resource = CraftResource.None;
                this.Hue = 1366;
            }
        }
    }
}