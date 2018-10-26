using System;

namespace Server.Items
{
    public class StaffOfPower : BlackStaff
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public StaffOfPower()
        {
            Hue = Utility.RandomBool() ? 0x4F2 : 0x4EF;
            WeaponAttributes.MageWeapon = 15;
            Attributes.SpellChanneling = 1;
            Attributes.SpellDamage = 5;
            Attributes.CastRecovery = 2;
            Attributes.LowerManaCost = 5;
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