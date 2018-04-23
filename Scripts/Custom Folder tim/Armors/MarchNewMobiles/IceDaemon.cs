using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of an Ice Daemon" )]
              public class IceDaemon : Daemon
              {
                                 [Constructable]
                                    public IceDaemon() : base()
                            {
                                               Name = "An Ice Daemon";
                                               Hue = 1152;
                                               //Body = 9; 
                                               //BaseSoundID = 357; 
                                               SetStr( 112 );
                                               SetDex( 95 );
                                               SetInt( 70 );
                                               SetHits( 400 );
                                               SetDamage( 30 );
                                               SetDamageType( ResistanceType.Physical, 5 );
                                               SetDamageType( ResistanceType.Cold, 80 );
                                               SetDamageType( ResistanceType.Fire, 5 );
                                               SetDamageType( ResistanceType.Energy, 5 );
                                               SetDamageType( ResistanceType.Poison, 5 );

                                               SetResistance( ResistanceType.Physical, 25 );
                                               SetResistance( ResistanceType.Cold, 100 );
                                               SetResistance( ResistanceType.Fire, 25 );
                                               SetResistance( ResistanceType.Energy, 25 );
                                               SetResistance( ResistanceType.Poison, 25 );
					       Fame = 1200;
                                               Karma = -700;
                                               VirtualArmor = 45;
     
                                               
                                               PackGold( 1000, 1020 );
                                               PackItem( new GreaterCurePotion() ); 

                            }
                                 public override bool HasBreath{ get{ return true ; } }
                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lesser ; } }

public IceDaemon( Serial serial ) : base( serial )
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
