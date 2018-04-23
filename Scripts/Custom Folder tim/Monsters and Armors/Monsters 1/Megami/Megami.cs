////////////////////////
//Designed by Neptune//
//////////////////////
using System;
using Server.Items;

namespace Server.Mobiles
              {

              [CorpseName( " corpse of Megami " )]
              public class Megami : BaseCreature
              {

		  public override WeaponAbility GetWeaponAbility()
		  {
			return Utility.RandomBool() ? WeaponAbility.CrushingBlow : WeaponAbility.MortalStrike;
		  }

                                 	[Constructable]
                                    public Megami() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
                            {
                                               Name = "Megami Tensei";
					       Title = "The Reborn Goddess";
                                               Hue = 2946;
                                               Body = 174;
					BaseSoundID = 0x3E9;
					RangeFight = 20;
					RangePerception = 20;
                                               SetStr( 6500 );
                                               SetDex( 3500 );
                                               SetInt( 1500 );
                                               SetHits( 100000 );
                                               SetDamage( 150 );
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

                                               SetSkill(SkillName.EvalInt, 200);
                                               SetSkill(SkillName.Tactics, 200);
                                               SetSkill(SkillName.MagicResist, 200);
                                               SetSkill(SkillName.Wrestling, 200);
                                               SetSkill(SkillName.Meditation, 200);
                                               SetSkill(SkillName.Focus, 200);
                                               SetSkill(SkillName.Magery, 200);
                                               SetSkill(SkillName.Anatomy, 200);
			
                                               Fame = -1000;
                                               Karma = -1000;
                                               Tamable = false;
                                               VirtualArmor = 87;


	    		NeptuneShirt Bracelet = new NeptuneShirt();
			Bracelet.Movable = false;
			AddItem(Bracelet);
                                            }
            
                                            public override void GenerateLoot()
                                            {
            	                               switch ( Utility.Random( 80 ))
			{
		case 0: PackItem( new MegamiChest() ); break;
		case 1: PackItem( new MegamiLegs() ); break;
		case 2: PackItem( new MegamiScepter() ); break;
                                            }}    

                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }
					   public override bool HasBreath{ get{ return true; } }

			    public Megami( Serial serial ) : base( serial )
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
