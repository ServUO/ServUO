using System;

namespace Server.Items
{
    public class DemonHuntersStandard : Spear
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113864; } } // Demon Hunter's Standard
		
        [Constructable]
        public DemonHuntersStandard()
        {
            Hue = 1377;				
            Attributes.CastSpeed = 1;			
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.HitLeechStam = 50;
            WeaponAttributes.HitLightning = 40;	
            WeaponAttributes.HitLowerDefend = 30;
            Slayer = SlayerName.Exorcism;
        }

        public DemonHuntersStandard(Serial serial)
            : base(serial)
        {
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}