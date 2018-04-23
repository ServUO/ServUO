////////////////////////////
//Designed by Needles Kane//
////////////////////////////
using System;
using Server.Items;

namespace Server.Mobiles
              {

              [CorpseName( " corpse of Chaos " )]
              public class Seth : BaseCreature
              {

		  public override WeaponAbility GetWeaponAbility()
		  {
			return Utility.RandomBool() ? WeaponAbility.Dismount : WeaponAbility.CrushingBlow;
		  }

                                 	[Constructable]
                                    public Seth() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
                            {
                                               Name = "Seth";
					       Title = "The God of Chaos";
                                               Hue = 2460;
                                               Body = 268;
					BaseSoundID = 0x3E9;
					RangeFight = 20;
					RangePerception = 20;
                                               SetStr( 666 );
                                               SetDex( 666 );
                                               SetInt( 666 );
                                               SetHits( 75000 );
                                               SetDamage( 666 );
                                               SetDamageType( ResistanceType.Physical, 0 );
                                               SetDamageType( ResistanceType.Cold, 100 );
                                               SetDamageType( ResistanceType.Fire, 100 );
                                               SetDamageType( ResistanceType.Energy, 100 );
                                               SetDamageType( ResistanceType.Poison, 100 );

                                               SetResistance( ResistanceType.Physical, 200 );
                                               SetResistance( ResistanceType.Cold, 200 );
                                               SetResistance( ResistanceType.Fire, 200 );
                                               SetResistance( ResistanceType.Energy, 200 );
                                               SetResistance( ResistanceType.Poison, 200 );

                                               SetSkill(SkillName.EvalInt, 666);
                                               SetSkill(SkillName.Tactics, 666);
                                               SetSkill(SkillName.MagicResist, 666);
                                               SetSkill(SkillName.Wrestling, 666);
                                               SetSkill(SkillName.Meditation, 666);
                                               SetSkill(SkillName.Focus, 666);
                                               SetSkill(SkillName.Magery, 666);
                                               SetSkill(SkillName.Anatomy, 666);
			
                                               Fame = -1000;
                                               Karma = -1000;
                                               Tamable = false;
                                               VirtualArmor = 666;
                                            }
            
                                            public override void GenerateLoot()
                                            {
            	                               switch ( Utility.Random( 80 ))
			{
		case 0: PackItem( new ChaosGloves() ); break;
		case 1: PackItem( new ChaosCloak() ); break;
		case 2: PackItem( new ChaosBoots() ); break;
		case 3: PackItem( new ChaosRobe() ); break;
		case 4: PackItem( new SlothChaosShield() ); break;
                                            }}    

                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }
					   public override bool HasBreath{ get{ return true; } }

			    public Seth( Serial serial ) : base( serial )
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
