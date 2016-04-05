using System;

namespace Server.Items
{
    public class ClawsOfTheBerserker : Tekagi
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ClawsOfTheBerserker()
            : base()
        {
            this.Name = ("Claws Of The Berserker");
		
            this.Hue = 1172;	
		
            this.WeaponAttributes.HitLightning = 45;	
            this.WeaponAttributes.HitLowerDefend = 50;
            this.WeaponAttributes.BattleLust = 1;
            this.Attributes.CastSpeed = 1;	
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 60;
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