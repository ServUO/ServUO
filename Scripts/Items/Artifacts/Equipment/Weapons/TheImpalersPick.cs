using System;

namespace Server.Items
{
    [FlipableAttribute(0x143D, 0x143C)]
    public class TheImpalersPick : HammerPick
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113822; } } // The Impaler's Pick
		
        [Constructable]
        public TheImpalersPick()
        {
            Hue = 2101;		
            WeaponAttributes.HitManaDrain = 10;
            Slayer = SlayerName.Repond;
            WeaponAttributes.HitLightning = 40;
            WeaponAttributes.HitLowerDefend = 40;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 45;	
        }

        public TheImpalersPick(Serial serial)
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