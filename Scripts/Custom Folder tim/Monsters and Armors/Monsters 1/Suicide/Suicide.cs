// Created by Neptune

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Suicide" )]
              public class Suicide : SkeletalDragon
              {
                                 [Constructable]
                                    public Suicide() : base()
                            {
                                               Name = "Suicide";
                                               Hue = 33775;
                                               Body = 400; // Uncomment these lines and input values
                                               //BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 500 );
                                               SetDex( 3000 );
                                               SetInt( 1500 );
                                               SetHits( 20000 );
                                               SetDamage( 25, 30 );
                                               SetDamageType( ResistanceType.Physical, 25 );
                                               SetDamageType( ResistanceType.Cold, 25 );
                                               SetDamageType( ResistanceType.Fire, 300 );
                                               SetDamageType( ResistanceType.Energy, 25 );
                                               SetDamageType( ResistanceType.Poison, 25 );

                                               SetResistance( ResistanceType.Physical, 70 );
                                               SetResistance( ResistanceType.Cold, 70 );
                                               SetResistance( ResistanceType.Fire, 70 );
                                               SetResistance( ResistanceType.Energy, 70 );
                                               SetResistance( ResistanceType.Poison, 70 );

						SetSkill(SkillName.Wrestling, 1500);		
				
                                               Fame = 30000;
                                               Karma = - 25000;
                                               VirtualArmor = 70;

						SuicideArms Arms = new SuicideArms();
					Arms.Movable = false;
					AddItem(Arms);
					
					SuicideChest Chest = new SuicideChest();
					Chest.Movable = false;
					AddItem(Chest);

					SuicideLegs Legs = new SuicideLegs();
					Legs.Movable = false;
					AddItem(Legs);

					SuicideGloves Gloves = new SuicideGloves();
					Gloves.Movable = false;
					AddItem(Gloves);

					SuicideGorget Gorget = new SuicideGorget();
					Gorget.Movable = false;
					AddItem(Gorget);
     
                                                 switch ( Utility.Random( 50 ))
			         {
				
				case 0: PackItem( new SuicideChest() ); break;
				case 1: PackItem( new SuicideLegs() ); break;
				case 2: PackItem( new SuicideArms() ); break;
				case 3: PackItem( new SuicideGloves() ); break;
				case 4: PackItem( new SuicideGorget() ); break;
                       		{	
			
			
		 }}

                            }

public Suicide( Serial serial ) : base( serial )
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
