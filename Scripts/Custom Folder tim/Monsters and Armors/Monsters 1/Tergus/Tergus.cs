// Created by Script Creator and CCL

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Tergus" )]
              public class Tergus : Daemon
              {
                                 [Constructable]
                                    public Tergus() : base()
                            {
                                               Name = "Tergus";
					       Title = "Master Crafter of Daemonic Leather";
                                               Hue = 33775;
                                               Body = 400;
                                               BaseSoundID = 679;
                                               SetStr( 2500 );
                                               SetDex( 2000 );
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
                                               VirtualArmor = 75;


					TergusArms Arms = new TergusArms();
					Arms.Movable = false;
					AddItem(Arms);
					
					TergusChest Chest = new TergusChest();
					Chest.Movable = false;
					AddItem(Chest);

					TergusLegs Legs = new TergusLegs();
					Legs.Movable = false;
					AddItem(Legs);

					TergusGloves Gloves = new TergusGloves();
					Gloves.Movable = false;
					AddItem(Gloves);

					TergusGorget Gorget = new TergusGorget();
					Gorget.Movable = false;
					AddItem(Gorget);

                                               PackGold( 5000 );
					       }
public override void GenerateLoot()
		{		
                                 switch ( Utility.Random( 60 ))
			         {
				case 0: PackItem( new TergusChest() ); break;
				case 1: PackItem( new TergusLegs() ); break;
				case 2: PackItem( new TergusArms() ); break;
				case 3: PackItem( new TergusGloves() ); break;
				case 4: PackItem( new TergusGorget() ); break;

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

public Tergus( Serial serial ) : base( serial )
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
