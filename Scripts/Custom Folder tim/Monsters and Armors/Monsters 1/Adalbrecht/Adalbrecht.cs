// Created by Neptune

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Adalbrecht" )]
              public class Adalbrecht : Centaur
              {
                                 [Constructable]
                                    public Adalbrecht() : base()
                            {
                                               Name = "Adalbrecht";
					       Title = "The Last of the Daminoc Clan";
                                               Hue = 33775;
                                               Body = 400;
                                               BaseSoundID = 679;
                                               SetStr( 250 );
                                               SetDex( 225 );
                                               SetInt( 200 );
                                               SetHits( 65000 );
                                               SetDamage( 20, 40 );
                                               SetDamageType( ResistanceType.Physical, 199 );
                                               SetDamageType( ResistanceType.Cold, 199 );
                                               SetDamageType( ResistanceType.Fire, 199 );
                                               SetDamageType( ResistanceType.Energy, 199 );
                                               SetDamageType( ResistanceType.Poison, 199 );

                                               SetResistance( ResistanceType.Physical, 99 );
                                               SetResistance( ResistanceType.Cold, 99 );
                                               SetResistance( ResistanceType.Fire, 99 );
                                               SetResistance( ResistanceType.Energy, 99 );
                                               SetResistance( ResistanceType.Poison, 99 );
                                               Fame = 12000;
                                               Karma = -1000;
                                               VirtualArmor = 5;

                                               PackGold( 5000 );

			DaminocChest Chest = new DaminocChest();
			Chest.Movable = false;
			AddItem(Chest);

			DaminocShield Shield = new DaminocShield();
			Shield.Movable = false;
			AddItem(Shield);
			
			DaminocLegs Legs = new DaminocLegs();
			Legs.Movable = false;
			AddItem(Legs);
			
			
			DaminocHelm Helm = new DaminocHelm();
			Helm.Movable = false;
			AddItem(Helm);

			DaminocBlade Weapon = new DaminocBlade();
			Weapon.Movable = false;
			AddItem(Weapon);

					       }
				public override void GenerateLoot()
		{		
                                 switch ( Utility.Random( 60 ))
			         {
				case 0: PackItem( new DaminocChest() ); break;
				case 1: PackItem( new DaminocLegs() ); break;
				case 2: PackItem( new DaminocHelm() ); break;
				case 3: PackItem( new DaminocBlade() ); break;
				case 4: PackItem( new DaminocShield() ); break;

                                                }}

                                 public override bool HasBreath{ get{ return true ; } }
				 public override int BreathFireDamage{ get{ return 9; } }
				 public override int BreathColdDamage{ get{ return 9; } }
                                 public override bool IsScaryToPets{ get{ return true; } }
				 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }

public Adalbrecht( Serial serial ) : base( serial )
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
