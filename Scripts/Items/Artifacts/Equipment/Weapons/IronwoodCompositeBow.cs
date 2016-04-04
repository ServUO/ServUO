using System;

namespace Server.Items
{
    public class IronwoodCompositeBow : CompositeBow
    {
        [Constructable]
        public IronwoodCompositeBow()
            : base()
        {
            this.Name = ("Ironwood Composite Bow");
		
            this.Hue = 1410;
			
            this.Slayer = SlayerName.Fey;
            this.WeaponAttributes.HitFireball = 40;
            this.WeaponAttributes.HitLowerDefend = 30;	
            this.Attributes.BonusDex = 5;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 45;
            this.Velocity = 30;
        }

        public IronwoodCompositeBow(Serial serial)
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}