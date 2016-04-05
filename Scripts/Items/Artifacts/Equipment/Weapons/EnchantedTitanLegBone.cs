using System;

namespace Server.Items
{
    public class EnchantedTitanLegBone : ShortSpear
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public EnchantedTitanLegBone()
        {
            this.Hue = 0x8A5;
            this.WeaponAttributes.HitLowerDefend = 40;
            this.WeaponAttributes.HitLightning = 40;
            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponDamage = 20;
            this.WeaponAttributes.ResistPhysicalBonus = 10;
        }

        public EnchantedTitanLegBone(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063482;
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