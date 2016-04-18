using System;

namespace Server.Items
{
    public class GlacialStaff : BlackStaff
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GlacialStaff()
        {
            this.Hue = 0x480;
            this.WeaponAttributes.HitHarm = 5 * Utility.RandomMinMax(1, 5);
            this.WeaponAttributes.MageWeapon = Utility.RandomMinMax(5, 10);

            this.AosElementDamages[AosElementAttribute.Cold] = 20 + (5 * Utility.RandomMinMax(0, 6));
        }

        public GlacialStaff(Serial serial)
            : base(serial)
        {
        }

        //TODO: Pre-AoS stuff
        public override int LabelNumber
        {
            get
            {
                return 1017413;
            }
        }// Glacial Staff
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}