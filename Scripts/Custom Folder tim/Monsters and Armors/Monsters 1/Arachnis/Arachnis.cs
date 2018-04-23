

using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Arachnis" )]
              public class Arachnis : BaseCreature
              {

                                 [Constructable]
                                    public Arachnis() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.2 )
                            {
                                               Name = "Arachnis";
					       Title = "Spider of the Golden Silk";
                                               Hue = 248;
								Paralyzed = false;
                                               Body = 28; // Uncomment these lines and input values
                                               //BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 5000, 7500 );
                                               SetDex( 3000, 5000 );
                                               SetInt( 1000, 2500 );
                                               SetHits( 150000, 250000 );
                                               SetDamage( 45, 55 );
                                               SetDamageType( ResistanceType.Cold, 120 );
                                               SetDamageType( ResistanceType.Fire, 120 );
                                               SetDamageType( ResistanceType.Energy, 120 );
                                               SetDamageType( ResistanceType.Poison, 120 );

                                               SetResistance( ResistanceType.Physical, 70 );
                                               SetResistance( ResistanceType.Cold, 80 );
                                               SetResistance( ResistanceType.Fire, 80 );
                                               SetResistance( ResistanceType.Energy, 20 );
                                               SetResistance( ResistanceType.Poison, 80 );



                                               Fame = 40000;
                                               Karma = -45000;
                                               VirtualArmor = 15;
		PackGold( 5120, 6130 );

			
}
public override void GenerateLoot()
		{		
			switch ( Utility.Random( 60 ))
			{
				case 0: PackItem( new SurcoatofArachnis() ); break;
				case 1: PackItem( new ShroudofArachnis() ); break;
				case 2: PackItem( new ShirtofArachnis() ); break;
                        case 3: PackItem( new SkirtofArachnis() ); break;
			case 4: PackItem( new CloakofArachnis() ); break;
			case 5: PackItem( new BootsofArachnis() ); break;
		  	case 6: PackItem( new SoulofArachnis() ); break;
                      
					{	
			
			
		 }}

                            }
                                
                               
                                 public override bool IsScaryToPets{ get{ return true; } }
				 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }
				




public Arachnis( Serial serial ) : base( serial )
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

