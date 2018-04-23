using System;
using Server;

namespace Server.Items
{ 
	public class TrainingMace: Mace
	{
		public override int LabelNumber{ get{ return 1061096; } } // Training Mace
		
        public override int InitMinHits{ get{ return 2600; } }
		public override int InitMaxHits{ get{ return 2600; } }
                
		public override int AosMinDamage{ get{ return 1; } }
		public override int AosMaxDamage{ get{ return 1; } }
		public override int AosSpeed{ get{ return 36; } }

		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 1; } }
		public override int OldSpeed{ get{ return 48; } }

		public override int DefHitSound{ get{ return 0x233; } }
		public override int DefMissSound{ get{ return 0x239; } }
	
	

		[Constructable]
		public TrainingMace()

		{
                        Name = "A Training Mace";
			Hue = 220;
			
                    Attributes.WeaponSpeed = 50;
		}

		public TrainingMace( Serial serial ) : base( serial )
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
