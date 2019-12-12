using System;

namespace Server.Items
{
    public class KatrinasCrook : ShepherdsCrook
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public KatrinasCrook()
            : base()
        {
            WeaponAttributes.HitLeechStam = 40;
            WeaponAttributes.HitLeechMana = 55;
            WeaponAttributes.HitLeechHits = 55;
            Attributes.WeaponDamage = 60;
            Attributes.DefendChance = 15;
        }

        public KatrinasCrook(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1079789;
            }
        }// Katrina's Crook
        public override int ArtifactRarity
        {
            get
            {
                return 11;
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

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
            {
                NegativeAttributes.NoRepair = 0;
            }
        }
    }
}