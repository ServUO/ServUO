////////////////////////
//Designed by Neptune//
//////////////////////
using System;
using Server.Items;

namespace Server.Mobiles
              {

              [CorpseName( " corpse of Destructabo Robo " )]
              public class DestRobo : BaseCreature
              {

		  public override WeaponAbility GetWeaponAbility()
		  {
			return Utility.RandomBool() ? WeaponAbility.Disarm : WeaponAbility.ArmorIgnore;
		  }

                                 	[Constructable]
                                    public DestRobo() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.4, 0.8 )
                            {
                                               Name = "Destructabo Robo";
                                               Hue = 1985;
                                               Body = 752;
					BaseSoundID = 0x3E9;

                                               SetStr( 900 );
                                               SetDex( 500 );
                                               SetInt( 250 );
                                               SetHits( 20000 );
                                               SetDamage( 75, 150 );
                                               SetDamageType( ResistanceType.Physical, 0 );
                                               SetDamageType( ResistanceType.Cold, 200 );
                                               SetDamageType( ResistanceType.Fire, 200 );
                                               SetDamageType( ResistanceType.Energy, 200 );
                                               SetDamageType( ResistanceType.Poison, 200 );

                                               SetResistance( ResistanceType.Physical, 200 );
                                               SetResistance( ResistanceType.Cold, 200 );
                                               SetResistance( ResistanceType.Fire, 200 );
                                               SetResistance( ResistanceType.Energy, 200 );
                                               SetResistance( ResistanceType.Poison, 200 );

                                               SetSkill(SkillName.EvalInt, 350);
                                               SetSkill(SkillName.Tactics, 350);
					       SetSkill(SkillName.Archery, 350);
                                               SetSkill(SkillName.MagicResist, 350);
                                               SetSkill(SkillName.Wrestling, 350);
                                               SetSkill(SkillName.Meditation, 350);
                                               SetSkill(SkillName.Focus, 350);
                                               SetSkill(SkillName.Magery, 350);
                                               SetSkill(SkillName.Anatomy, 350);
			
                                               Fame = -1000;
                                               Karma = -1000;
                                               Tamable = false;
                                               VirtualArmor = 87;


	    		NDRoboBlaster Weapon = new NDRoboBlaster();
			Weapon.Movable = false;
			AddItem(Weapon);
                                            }
            
                                            public override void GenerateLoot()
                                            {
            	                               switch ( Utility.Random( 80 ))
			{
		case 0: PackItem( new RoboArms() ); break;
		case 1: PackItem( new RoboLegs() ); break;
		case 2: PackItem( new RoboChest() ); break;
		case 3: PackItem( new RoboGorget() ); break;
		case 4: PackItem( new DRoboBlaster() ); break;
		case 5: PackItem( new RoboGloves() ); break;
		case 6: PackItem( new RoboHelm() ); break;
                                            }
                                 switch ( Utility.Random( 0 ))
			         {
				case 0: PackItem( new BlasterAmmo( 20000 ) ); break;
					}
			}   

                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }
					   public override bool HasBreath{ get{ return true; } }

			    public DestRobo( Serial serial ) : base( serial )
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
