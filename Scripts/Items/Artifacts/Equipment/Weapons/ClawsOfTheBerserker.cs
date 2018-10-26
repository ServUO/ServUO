using System;

namespace Server.Items
{
    public class ClawsOfTheBerserker : Tekagi
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113758; } } // Claws of the Berserker
		
        [Constructable]
        public ClawsOfTheBerserker()
            : base()
        {
            Hue = 1172;		
            WeaponAttributes.HitLightning = 45;	
            WeaponAttributes.HitLowerDefend = 50;
            WeaponAttributes.BattleLust = 1;
            Attributes.CastSpeed = 1;	
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 60;
        }

        public ClawsOfTheBerserker(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 35;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 60;
            }
        }
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