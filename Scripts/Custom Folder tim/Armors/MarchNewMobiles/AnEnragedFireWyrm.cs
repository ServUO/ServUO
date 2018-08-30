using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a An Enraged Fire Wyrm" )]
              public class AnEnragedFireWyrm : WhiteWyrm
              {
                                 [Constructable]
                                    public AnEnragedFireWyrm() : base()
                            {
                                               Name = "An Enraged Fire Wyrm";
                                               Hue = 1174;
                                               Body = 46; 
                                               BaseSoundID = 362; 
                                               SetStr( 101 );
                                               SetDex( 50 );
                                               SetInt( 80 );
                                               SetHits( 800 );
                                               SetDamage( 50 );
                                               SetDamageType( ResistanceType.Physical, 5 );
                                               SetDamageType( ResistanceType.Cold, 5 );
                                               SetDamageType( ResistanceType.Fire, 80 );
                                               SetDamageType( ResistanceType.Energy, 5 );
                                               SetDamageType( ResistanceType.Poison, 5 );

                                               SetResistance( ResistanceType.Physical, 25 );
                                               SetResistance( ResistanceType.Cold, 25 );
                                               SetResistance( ResistanceType.Fire, 100 );
                                               SetResistance( ResistanceType.Energy, 25 );
                                               SetResistance( ResistanceType.Poison, 25 );
                                               Fame = 100;
                                               Karma = -3200;
                                               VirtualArmor = 45;
     
                                               Tamable = true;
                                               ControlSlots = 2;
                                               MinTameSkill = 110;
                                               PackGold( 5000,10000 );
                                               

                            }
                                 public override bool HasBreath{ get{ return true ; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }

public AnEnragedFireWyrm( Serial serial ) : base( serial )
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
