using System;

namespace Server.Items
{
    public class NightsKiss : Dagger
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public NightsKiss()
        {
            Hue = 0x455;
            WeaponAttributes.HitLeechHits = 40;
            Slayer = SlayerName.Repond;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 35;
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