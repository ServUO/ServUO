using System;

namespace Server.Items
{
    public class BladeDance : RuneBlade
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BladeDance()
        {
            this.Hue = 0x66C;

            this.Attributes.BonusMana = 8;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.WeaponDamage = 30;
            this.WeaponAttributes.HitLeechMana = 20;
            this.WeaponAttributes.UseBestSkill = 1;
        }

        public BladeDance(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075033;
            }
        }// Blade Dance
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}