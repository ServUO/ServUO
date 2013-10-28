using System;
using Server.Mobiles;

namespace Server.Items
{
	public class SerpentNest : Item
	{
		[Constructable]
		public SerpentNest() : base( 0x2233 )
		{
			LootType = LootType.Regular;
			Weight = 1.0;
			Hue = 0;
            		Name = ("Serpent Nest");
            		Movable = false;
		}

		public override void OnDoubleClick( Mobile from )
		{
			SerpentNest nest = (SerpentNest)this;
			from.Animate( 32, 5, 1, true, false, 0 );
			if ( Utility.RandomDouble() < 0.20 )  //% may be off, just a rough guess
			switch (Utility.Random(4))
			{
				case 0: 
				{
					RareSerpentEgg4 RSEB = new RareSerpentEgg4();
					RSEB.MoveToWorld(new Point3D(((SerpentNest)this).X, ((SerpentNest)this).Y, ((SerpentNest)this).Z), ((SerpentNest)this).Map);
					from.SendLocalizedMessage( 1112581 ); // You reach in and find a rare serpent egg!!
					nest.Delete();	
				} break;
				case 1: 
				{
					RareSerpentEgg3 RSEW = new RareSerpentEgg3();
					RSEW.MoveToWorld(new Point3D(((SerpentNest)this).X, ((SerpentNest)this).Y, ((SerpentNest)this).Z), ((SerpentNest)this).Map);
					from.SendLocalizedMessage( 1112581 ); // You reach in and find a rare serpent egg!!	
					nest.Delete();	
				} break;
				case 2: 
				{
					RareSerpentEgg2 RSER = new RareSerpentEgg2();
					RSER.MoveToWorld(new Point3D(((SerpentNest)this).X, ((SerpentNest)this).Y, ((SerpentNest)this).Z), ((SerpentNest)this).Map);
					from.SendLocalizedMessage( 1112581 ); // You reach in and find a rare serpent egg!!	
					nest.Delete();	
				} break;
				case 3: 
				{
					RareSerpentEgg1 RSEY = new RareSerpentEgg1();
					RSEY.MoveToWorld(new Point3D(((SerpentNest)this).X, ((SerpentNest)this).Y, ((SerpentNest)this).Z), ((SerpentNest)this).Map);
					from.SendLocalizedMessage( 1112581 ); // You reach in and find a rare serpent egg!!		
					nest.Delete();	
				} break;
			}
			else if ( Utility.RandomDouble() >= 0.20 )
			switch (Utility.Random(4))
			{
				case 0: 
				{
					from.SendLocalizedMessage( 1112578 ); // You try to reach the eggs, but the hole is too deep.
				} break;
				case 1: 
				{
					CoralSnake S1 = new CoralSnake();  //Not sure of what type or how many snakes it spawns
					CoralSnake S3 = new CoralSnake();  //Not sure of what type or how many snakes it spawns
					S1.MoveToWorld(new Point3D(((SerpentNest)this).X, ((SerpentNest)this).Y, ((SerpentNest)this).Z), ((SerpentNest)this).Map);
					S3.MoveToWorld(new Point3D(((SerpentNest)this).X, ((SerpentNest)this).Y, ((SerpentNest)this).Z), ((SerpentNest)this).Map);
					from.SendLocalizedMessage( 1112577 ); // A swarm of snakes springs forth from the nest and attacks you!!!							
				} break;
				case 2: 
				{
					LavaSnake S2 = new LavaSnake();  //Not sure of what type or how many snakes it spawns
					LavaSnake S4 = new LavaSnake();  //Not sure of what type or how many snakes it spawns
					S2.MoveToWorld(new Point3D(((SerpentNest)this).X, ((SerpentNest)this).Y, ((SerpentNest)this).Z), ((SerpentNest)this).Map);
					S4.MoveToWorld(new Point3D(((SerpentNest)this).X, ((SerpentNest)this).Y, ((SerpentNest)this).Z), ((SerpentNest)this).Map);
					from.SendLocalizedMessage( 1112577 ); // A swarm of snakes springs forth from the nest and attacks you!!!								
				} break;
				case 3: 
				{
					from.SendLocalizedMessage( 1112579 ); // You reach in but clumsily destroy the eggs inside the nest.		
					nest.Delete();							
				} break;
			}			
		}
		public SerpentNest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
            
            		if ( Movable == true )
                	Movable = false;
		}
	}
}

