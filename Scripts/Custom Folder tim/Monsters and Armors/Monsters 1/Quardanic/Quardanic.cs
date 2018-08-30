// Created by Dudus

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Quardanic" )]
              public class Quardanic : Daemon
              {
                                 [Constructable]
                                    public Quardanic() : base()
                            {
                                               Name = "Quardanic";
					       Title = "The Ancient Dragon Lord";
                                               Hue = 33775;
                                               Body = 400; // Uncomment these lines and input values
                                               //BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 3000 );
                                               SetDex( 9000 );
                                               SetInt( 6000 );
                                               SetHits( 80000 );
                                               SetDamage( 25, 50 );
                                               SetDamageType( ResistanceType.Physical, 100 );
                                               SetDamageType( ResistanceType.Cold, 60 );
                                               SetDamageType( ResistanceType.Fire, 60 );
                                               SetDamageType( ResistanceType.Energy, 60 );
                                               SetDamageType( ResistanceType.Poison, 60 );

                                               SetResistance( ResistanceType.Physical, 70 );
                                               SetResistance( ResistanceType.Cold, 70 );
                                               SetResistance( ResistanceType.Fire, 70 );
                                               SetResistance( ResistanceType.Energy, 70 );
                                               SetResistance( ResistanceType.Poison, 70 );
                                               Fame = 30000;
                                               Karma = - 25000;
                                               VirtualArmor = 70;

					new EtherealSwampDragon().Rider = this;

			Item FancyShirt = new FancyShirt(); 
			FancyShirt.Movable = false;
			FancyShirt.Hue = 1; 
			AddItem( FancyShirt );

			Item LongPants = new LongPants(); 
			LongPants.Movable = false;
			LongPants.Hue = 1; 
			AddItem( LongPants );

			Item Cloak = new Cloak(); 
			Cloak.Movable = false;
			Cloak.Hue = 1; 
			AddItem( Cloak );

			Item Boots = new Boots(); 
			Boots.Movable = false;
			Boots.Hue = 1; 
			AddItem( Boots );
     
                                                 switch ( Utility.Random( 35 ))
			         {
				
				case 1: PackItem( new AncientDragonLordChest() ); break;
				case 2: PackItem( new AncientDragonLordGloves() ); break;
                        	case 3: PackItem( new AncientDragonLordHelm() ); break;
                        	case 4: PackItem( new AncientDragonLordArms() ); break;
                        	case 5: PackItem( new AncientDragonLordLegs() ); break;
                       		{	
			
			
		 }}

                            }
		public override bool CanRummageCorpses{ get{ return true; } }
        public override bool BardImmune{ get{ return true; } }
        public override Poison HitPoison{ get{ return Poison.Lethal; } }
        public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
        public override int TreasureMapLevel{ get{ return 5; } }

public Quardanic( Serial serial ) : base( serial )
                      {
                      }

public override void Serialize( GenericWriter writer )
                      {
                                        base.Serialize( writer );
                                        writer.Write( (int) 0 );
                      }

        public override void Deserialize( GenericReader reader )
                      {
                                        base.Deserialize( reader );
                                        int version = reader.ReadInt();
                      }
    }
}
