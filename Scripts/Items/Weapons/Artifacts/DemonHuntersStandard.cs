using System;

namespace Server.Items
{
    public class DemonHuntersStandard : Spear
    {
        [Constructable]
        public DemonHuntersStandard()
            : base(0xF62)
        {
            this.Name = ("Demon Hunter's Standard");
		
            this.Hue = 1377;	
			
            this.Attributes.CastSpeed = 1;			
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 50;
            this.WeaponAttributes.HitLeechStam = 50;
            this.WeaponAttributes.HitLightning = 40;	
            this.WeaponAttributes.HitLowerDefend = 30;
            this.Slayer = SlayerName.DaemonDismissal;
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