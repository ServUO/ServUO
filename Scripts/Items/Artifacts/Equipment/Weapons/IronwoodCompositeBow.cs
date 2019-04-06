using System;

namespace Server.Items
{
    public class IronwoodCompositeBow : CompositeBow
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113759; } } // Ironwood Composite Bow
		
        [Constructable]
        public IronwoodCompositeBow()
            : base()
        {
            Hue = 1410;			
            Slayer = SlayerName.Fey;
            WeaponAttributes.HitFireball = 40;
            WeaponAttributes.HitLowerDefend = 30;	
            Attributes.BonusDex = 5;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 45;
            Velocity = 30;
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