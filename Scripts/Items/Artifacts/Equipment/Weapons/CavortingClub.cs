using System;

namespace Server.Items
{
    public class CavortingClub : Club
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public CavortingClub()
        {
            this.Hue = 0x593;
            this.WeaponAttributes.SelfRepair = 3;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 35;
            this.WeaponAttributes.ResistFireBonus = 8;
            this.WeaponAttributes.ResistColdBonus = 8;
            this.WeaponAttributes.ResistPoisonBonus = 8;
            this.WeaponAttributes.ResistEnergyBonus = 8;
        }

        public CavortingClub(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063472;
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