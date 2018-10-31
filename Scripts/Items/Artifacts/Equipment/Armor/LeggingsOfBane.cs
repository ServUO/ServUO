using System;

namespace Server.Items
{
    public class LeggingsOfBane : ChainLegs
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LeggingsOfBane()
        {
            Hue = 0x4F5;
            ArmorAttributes.DurabilityBonus = 100;
            Attributes.BonusStam = 8;
            Attributes.AttackChance = 20;
        }

        public LeggingsOfBane(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061100;
            }
        }// Leggings of Bane
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 36;
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

            writer.Write((int)2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version <= 1)
            {
                if (this.HitPoints > 255 || this.MaxHitPoints > 255)
                    this.HitPoints = this.MaxHitPoints = 255;
            }

            if (version < 1)
            {
                if (this.Hue == 0x559)
                    this.Hue = 0x4F5;

                if (this.ArmorAttributes.DurabilityBonus == 0)
                    this.ArmorAttributes.DurabilityBonus = 100;

                this.PoisonBonus = 0;
            }
        }
    }
}