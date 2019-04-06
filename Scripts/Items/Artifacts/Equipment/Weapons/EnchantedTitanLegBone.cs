using System;

namespace Server.Items
{
    public class EnchantedTitanLegBone : ShortSpear
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public EnchantedTitanLegBone()
        {
            Hue = 0x8A5;
            WeaponAttributes.HitLowerDefend = 40;
            WeaponAttributes.HitLightning = 40;
            Attributes.AttackChance = 10;
            Attributes.WeaponDamage = 20;
            WeaponAttributes.ResistPhysicalBonus = 10;
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