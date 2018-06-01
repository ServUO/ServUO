using System;

namespace Server.Items
{
    public class DeathsHead : DiscMace
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DeathsHead() 
        {
            Name = ("Death's Head");
		
            Hue = 1154;	
            WeaponAttributes.HitFatigue = 10;
            WeaponAttributes.HitLightning = 45;	
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 45;
        }

        public DeathsHead(Serial serial)
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
