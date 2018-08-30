using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a Fire Wyrm" )]
              public class FireWyrm : WhiteWyrm
              {
                                 [Constructable]
                                    public FireWyrm() : base()
                            {
                                               Name = "A Fire Wyrm";
                                               Hue = 1174;
                                               //Body = 180; 
                                               //BaseSoundID = 362; 
                                               SetStr( 98 );
                                               SetDex( 77 );
                                               SetInt( 61 );
                                               SetHits( 700 );
                                               SetDamage( 35 );
                                               SetDamageType( ResistanceType.Physical, 25 );
                                               SetDamageType( ResistanceType.Cold, 0 );
                                               SetDamageType( ResistanceType.Fire, 60 );
                                               SetDamageType( ResistanceType.Energy, 15 );
                                               SetDamageType( ResistanceType.Poison, 0 );

                                               SetResistance( ResistanceType.Physical, 75 );
                                               SetResistance( ResistanceType.Cold, 5 );
                                               SetResistance( ResistanceType.Fire, 120 );
                                               SetResistance( ResistanceType.Energy, 75 );
                                               SetResistance( ResistanceType.Poison, 75 );
                                               Fame = 25000;
                                               Karma = -12000;
                                               VirtualArmor = 35;
     
                                               
                                               
                                               
                                               PackGold( 1500, 1500 );
                                               

                            }
                                 public override bool HasBreath{ get{ return true ; } }
                                 public override bool Unprovokable{ get{ return true; } }

public FireWyrm( Serial serial ) : base( serial )
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
