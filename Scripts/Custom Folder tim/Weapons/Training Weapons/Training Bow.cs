using System;
using Server;

namespace Server.Items
{
	public class TrainingBow : Bow
	{
	        public override int LabelNumber{ get{ return 1061094; } } // Training Bow
                
                public override int InitMinHits{ get{ return 2600; } }
                public override int InitMaxHits{ get{ return 2600; } }

		public override int AosMinDamage{ get{ return 1; } }
		public override int AosMaxDamage{ get{ return 1; } }
		public override int AosSpeed{ get{ return 46; } }

		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 1; } }
		public override int OldSpeed{ get{ return 58; } }

		public override int DefMaxRange{ get{ return 15; } }

		public override int DefHitSound{ get{ return 0x234; } }
		public override int DefMissSound{ get{ return 0x238; } }

        
        
		
                [Constructable]
		public TrainingBow()
		{
			
			Hue = 220;
			Name = "A Training Bow";

            Attributes.WeaponSpeed = 50;
          
        }

		

		public TrainingBow( Serial serial ) : base( serial )
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
