// Created by Neptune

using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of the Revelation" )]
              public class Revelation : BaseCreature
              {

                                 [Constructable]
                                    public Revelation() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.2 )
                            {
                                               Name = "Apocalypsis";
					       Title = "The Revelation";
                                               Hue = 1700;
								Paralyzed = false;
                                               Body = 123; // Uncomment these lines and input values
                                               BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 2000, 2500 );
                                               SetDex( 1500, 2000 );
                                               SetInt( 1500, 2000 );
                                               SetHits( 200000, 250000 );
                                               SetDamage( 25, 30 );
                                               SetDamageType( ResistanceType.Cold, 120 );
                                               SetDamageType( ResistanceType.Fire, 120 );
                                               SetDamageType( ResistanceType.Energy, 120 );
                                               SetDamageType( ResistanceType.Poison, 120 );

                                               SetResistance( ResistanceType.Physical, 70 );
                                               SetResistance( ResistanceType.Cold, 80 );
                                               SetResistance( ResistanceType.Fire, 80 );
                                               SetResistance( ResistanceType.Energy, 80 );
                                               SetResistance( ResistanceType.Poison, 80 );

			SetSkill( SkillName.EvalInt, 320.1, 330.0 );
			SetSkill( SkillName.Magery, 290.1, 300.0 );
			SetSkill( SkillName.Meditation, 200.1, 301.0 );
			SetSkill( SkillName.Poisoning, 200.1, 301.0 );
			SetSkill( SkillName.MagicResist, 575.2, 600.0 );
			SetSkill( SkillName.Tactics, 390.1, 400.0 );
			SetSkill( SkillName.Wrestling, 375.1, 400.0 );
			SetSkill( SkillName.Swords, 375.1, 400.0 );
			SetSkill( SkillName.Anatomy, 375.1, 400.0 );
			SetSkill( SkillName.Parry, 250.1, 300.0 );


                                               Fame = 40000;
                                               Karma = -45000;
                                               VirtualArmor = 70;
		PackGold( 11120, 11130 );

}
public override void GenerateLoot()
		{		
			switch ( Utility.Random( 75 ))
			{
				
				case 0: PackItem( new RevelationShirt() ); break;
				case 1: PackItem( new RevelationSkirt() ); break;
				case 2: PackItem( new RevelationSash() ); break;
				case 3: PackItem( new RevelationRobe() ); break;
				case 4: PackItem( new RevelationBoots() ); break;
				case 5: PackItem( new RevelationCloak() ); break;
				case 6: PackItem( new RevelationApron() ); break;
				

							
		 }

                            }

                                 public override bool IsScaryToPets{ get{ return true; } }
				 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }
				



		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)from;

				if ( bc.Controlled || bc.BardTarget == this )
					damage = 0; // Immune to pets and provoked creatures
			}
		}



public Revelation( Serial serial ) : base( serial )
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

