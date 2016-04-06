using System;

namespace Server.Items
{
    public class RighteousAnger : ElvenMachete
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RighteousAnger()
        {
            this.Hue = 0x284;

            this.Attributes.AttackChance = 15;
            this.Attributes.DefendChance = 5;
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 40;
        }

        public RighteousAnger(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075049;
            }
        }// Righteous Anger
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