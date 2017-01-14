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
            this.WeaponAttributes.HitLeechStam = 40;
            this.WeaponAttributes.HitLeechMana = 55;
            this.WeaponAttributes.HitLeechHits = 55;

            this.Attributes.WeaponDamage = 60;
            this.Attributes.DefendChance = 15;
            this.BlockRepair = true;
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
                return 120;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 120;
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