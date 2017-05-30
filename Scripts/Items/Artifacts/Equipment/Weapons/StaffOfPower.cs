using System;

namespace Server.Items
{
    public class StaffOfPower : BlackStaff
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public StaffOfPower()
        {
            this.Hue = Utility.RandomBool() ? 0x4F2 : 0x4EF;
            this.WeaponAttributes.MageWeapon = 15;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.SpellDamage = 5;
            this.Attributes.CastRecovery = 2;
            this.Attributes.LowerManaCost = 5;
        }

        public StaffOfPower(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070692;
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