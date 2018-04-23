using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a Baby Fire Wyrm" )]
              public class BabyFireWyrm : Drake
              {
                                 [Constructable]
                                    public BabyFireWyrm() : base()
                            {
                                               Name = "A Baby Fire Wyrm";
                                               Hue = 1174;
                                               Body = 60; 
                                               BaseSoundID = 362; 
                                               SetStr( 89 );
                                               SetDex( 78 );
                                               SetInt( 100 );
                                               SetHits( 600 );
                                               SetDamage( 21 );
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
                                               Fame = 200;
                                               Karma = -500;
                                               VirtualArmor = 25;
     
                                               
                                               ControlSlots = 1;
                                               MinTameSkill = 80;
                                               PackGold( 500, 1000 );
                                                

                            }
                                 public override bool HasBreath{ get{ return true ; } }

public BabyFireWyrm( Serial serial ) : base( serial )
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
