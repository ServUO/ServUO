using System;

namespace Server.Items
{
    public class TalonBite : OrnateAxe
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TalonBite()
        {
            this.ItemID = 0x2D34;
            this.Hue = 0x47E;

            this.SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);

            this.Attributes.BonusDex = 8;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 35;

            this.WeaponAttributes.HitHarm = 33;
            this.WeaponAttributes.UseBestSkill = 1;
        }

        public TalonBite(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075029;
            }
        }// Talon Bite
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