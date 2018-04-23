////////////////////////////
//Designed by Needles Kane//
////////////////////////////
using System;
using Server.Items;

namespace Server.Mobiles
              {

              [CorpseName( " corpse of Sin " )]
              public class Peccatus : BaseCreature
              {

		  public override WeaponAbility GetWeaponAbility()
		  {
			return Utility.RandomBool() ? WeaponAbility.Dismount : WeaponAbility.CrushingBlow;
		  }

                                 	[Constructable]
                                    public Peccatus() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
                            {
                                               Name = "Septem Mactabilis Peccatus";
					       Title = "The Physical Embodiment of the Seven Deadly Sins";
                                               Hue = 2100;
                                               Body = 259;
					BaseSoundID = 0x3E9;

                                               SetStr( 7666 );
                                               SetDex( 7666 );
                                               SetInt( 7666 );
                                               SetHits( 666666 );
                                               SetDamage( 150, 200 );
                                               SetDamageType( ResistanceType.Physical, 0 );
                                               SetDamageType( ResistanceType.Cold, 666 );
                                               SetDamageType( ResistanceType.Fire, 666 );
                                               SetDamageType( ResistanceType.Energy, 666 );
                                               SetDamageType( ResistanceType.Poison, 666 );

                                               SetResistance( ResistanceType.Physical, 666 );
                                               SetResistance( ResistanceType.Cold, 666 );
                                               SetResistance( ResistanceType.Fire, 666 );
                                               SetResistance( ResistanceType.Energy, 666 );
                                               SetResistance( ResistanceType.Poison, 666 );

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
		case 0: PackItem( new SinShield() ); break;
		case 1: PackItem( new SinChest() ); break;
		case 2: PackItem( new SinLegs() ); break;
		case 3: PackItem( new SinArms() ); break;
		case 4: PackItem( new SinGloves() ); break;
		case 5: PackItem( new SinHelm() ); break;
		case 6: PackItem( new SinBlade() ); break;
                                            }}    

                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }
					   public override bool HasBreath{ get{ return true; } }

			    public Peccatus( Serial serial ) : base( serial )
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
