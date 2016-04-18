using System;

namespace Server.Items
{
    public class NightsKiss : Dagger
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public NightsKiss()
        {
            this.ItemID = 0xF51;
            this.Hue = 0x455;
            this.WeaponAttributes.HitLeechHits = 40;
            this.Slayer = SlayerName.Repond;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 35;
        }

        public NightsKiss(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063475;
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