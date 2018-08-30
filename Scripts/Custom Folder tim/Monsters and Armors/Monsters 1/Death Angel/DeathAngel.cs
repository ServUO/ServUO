// Created by Neptune

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Death Angel" )]
              public class DeathAngel : ChaosDaemon
              {
                                 [Constructable]
                                    public DeathAngel() : base()
                            {
                                               Name = "Nex Angelus";
					       Title = "The Death Angel";
                                               Hue = 33918;
                                               Body = 401;
                                               BaseSoundID = 679;
                                               SetStr( 2500 );
                                               SetDex( 2250 );
                                               SetInt( 2000 );
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
                                               VirtualArmor = 50;

                                               PackGold( 5000 );

			DAChest Chest = new DAChest();
			Chest.Movable = false;
			AddItem(Chest);

			DASkirt Legs = new DASkirt();
			Legs.Movable = false;
			AddItem(Legs);
			
			DACirclet Circlet = new DACirclet();
			Circlet.Movable = false;
			AddItem(Circlet);
			
			DASandals Sandals = new DASandals();
			Sandals.Movable = false;
			AddItem(Sandals);
			
			DASickle Weapon = new DASickle();
			Weapon.Movable = false;
			AddItem(Weapon);

					       }
				public override void GenerateLoot()
		{		
                                 switch ( Utility.Random( 60 ))
			         {
				case 0: PackItem( new DASickle() ); break;
				case 1: PackItem( new DASkirt() ); break;
				case 2: PackItem( new DAChest() ); break;
				case 3: PackItem( new DASandals() ); break;
				case 4: PackItem( new DACirclet() ); break;

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

public DeathAngel( Serial serial ) : base( serial )
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
