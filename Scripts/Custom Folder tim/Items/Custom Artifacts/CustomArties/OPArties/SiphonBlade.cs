using System;
using Server;

namespace Server.Items
{
	public class SiphonBlade : CrescentBlade, ITokunoDyable
	{

        public override int ArtifactRarity { get { return 19; } }
		
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public SiphonBlade()
		{
			Hue = 437;
			Name = "Siphon";
			
			Attributes.BonusDex = 5;
			Attributes.AttackChance = 10;
			WeaponAttributes.HitLeechMana = 35;
			WeaponAttributes.HitLeechHits = 35;
			WeaponAttributes.HitLeechStam = 35;
			Attributes.WeaponDamage = 50;
			Attributes.WeaponSpeed = 20;
		}

		public SiphonBlade( Serial serial ) : base( serial )
		{
		}
		
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
        }
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		}
	}
}
