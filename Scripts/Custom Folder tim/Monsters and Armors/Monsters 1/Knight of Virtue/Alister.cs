// Created by Dudus

using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of the Virtuous Knight" )]
              public class Alister : BaseCreature
              {

                                 [Constructable]
                                    public Alister() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.2 )
                            {
                                               Name = "Alister";
					       Title = "Knight of Virtue";
                                               Hue = 33775;
								Paralyzed = false;
                                               Body = 400; // Uncomment these lines and input values
                                               //BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 5500 );
                                               SetDex( 4500 );
                                               SetInt( 3500 );
                                               SetHits( 100000, 150000 );
                                               SetDamage( 45, 55 );
                                               SetDamageType( ResistanceType.Cold, 120 );
                                               SetDamageType( ResistanceType.Fire, 120 );
                                               SetDamageType( ResistanceType.Energy, 120 );
                                               SetDamageType( ResistanceType.Poison, 120 );

                                               SetResistance( ResistanceType.Physical, 70 );
                                               SetResistance( ResistanceType.Cold, 80 );
                                               SetResistance( ResistanceType.Fire, 80 );
                                               SetResistance( ResistanceType.Energy, 80 );
                                               SetResistance( ResistanceType.Poison, 80 );

			SetSkill( SkillName.EvalInt, 120.0 );
			SetSkill( SkillName.Magery, 120.0 );
			SetSkill( SkillName.Meditation, 120.0 );
			SetSkill( SkillName.Poisoning, 120.0 );
			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Swords, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0 );
			SetSkill( SkillName.Parry, 120.0 );


                                               Fame = 40000;
                                               Karma = -45000;
                                               VirtualArmor = 70;

					VirtueArms Arms = new VirtueArms();
					Arms.Movable = false;
					AddItem(Arms);
					
					VirtueChest Chest = new VirtueChest();
					Chest.Movable = false;
					AddItem(Chest);

					VirtueLegs Legs = new VirtueLegs();
					Legs.Movable = false;
					AddItem(Legs);

					VirtueGloves Gloves = new VirtueGloves();
					Gloves.Movable = false;
					AddItem(Gloves);

					VirtueGorget Gorget = new VirtueGorget();
					Gorget.Movable = false;
					AddItem(Gorget);

					VirtueShield Shield = new VirtueShield();
					Shield.Movable = false;
					AddItem(Shield);

					VirtueShoes Shoes = new VirtueShoes();
					Shoes.Movable = false;
					AddItem(Shoes);

					VirtueCloak Cloak = new VirtueCloak();
					Cloak.Movable = false;
					AddItem(Cloak);

					VirtueBlade Weapon = new VirtueBlade();
					Weapon.Movable = false;
					AddItem(Weapon);


		PackGold( 11120, 11130 );
}
public override void GenerateLoot()
		{		
                                 switch ( Utility.Random( 60 ))
			         {
				case 0: PackItem( new VirtueChest() ); break;
				case 1: PackItem( new VirtueLegs() ); break;
				case 2: PackItem( new VirtueArms() ); break;
				case 3: PackItem( new VirtueGloves() ); break;
				case 4: PackItem( new VirtueHelm() ); break;
				case 5: PackItem( new VirtueGorget() ); break;
				case 6: PackItem( new VirtueBlade() ); break;
				case 7: PackItem( new VirtueCloak() ); break;
				case 8: PackItem( new VirtueShield() ); break;
				case 9: PackItem( new VirtueShoes() ); break;
							
		 }
                               

                            }

                                 public override bool HasBreath{ get{ return true ; } }
				 public override int BreathFireDamage{ get{ return 9; } }
				 public override int BreathColdDamage{ get{ return 9; } }
                                 public override bool IsScaryToPets{ get{ return true; } }
				 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }
				



			public Alister( Serial serial ) : base( serial )
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

