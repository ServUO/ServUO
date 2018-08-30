/*********************************************************************
 * 
 * The original weapon pack was posted to RunUO on 18AUG2005 by Triple. 
 * http://www.runuo.com/community/threads/training-weapons.57430/ 
 * 
 * It included: Training Katana, Training Kryss, Training Mace, and Training Bow.
 * 
 * I added the Training Buckler, Training Gargish Boomerang, Training Gargish Kryss,
 * Training Gargish Mace, Training Gargish Shield, and Training Gargish Sword. 
 * 
 * I had to work on them some to get all the Special Abilities to work. Now you can use 
 * your specials to get your mana down, so you can work your Meditation and Focus while 
 * training fighting skills. When used in conjunction with Training Elementals you can 
 * set up a nice training area. Hopefully you will find this script in a package with the set.
 * 
 * Tukaram 21MAY2016
 * 
 * ********************************************************************/

using System;
using Server;

namespace Server.Items
{ 
	public class TrainingBuckler: Buckler
	{
		public override int LabelNumber{ get{ return 1061097; } } // Training Buckler
		
                public override int InitMinHits{ get{ return 2600; } }
		public override int InitMaxHits{ get{ return 2600; } }

        public override int ArmorBase{get { return 10; } }

      	

		[Constructable]
		public TrainingBuckler()

		{
                        Name = "A Training Buckler";
			Hue = 220;
	                        
		}

		public TrainingBuckler( Serial serial ) : base( serial )
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
