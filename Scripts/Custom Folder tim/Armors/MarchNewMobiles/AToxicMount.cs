using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a A Toxic Mount" )]
              public class AToxicMount : Horse
              {
                                 [Constructable]
                                    public AToxicMount() : base()
                            {
                                               Name = "A Toxic Mount";
                                               Hue = 1272;
                                               Body = 793; 
                                               BaseSoundID = 0xA8; 
                                               SetStr( 95 );
                                               SetDex( 100 );
                                               SetInt( 60 );
                                               SetHits( 210 );
                                               SetDamage( 30 );
                                               SetDamageType( ResistanceType.Physical, 5 );
                                               SetDamageType( ResistanceType.Cold, 5 );
                                               SetDamageType( ResistanceType.Fire, 5 );
                                               SetDamageType( ResistanceType.Energy, 5 );
                                               SetDamageType( ResistanceType.Poison, 80 );

                                               SetResistance( ResistanceType.Physical, 25 );
                                               SetResistance( ResistanceType.Cold, 25 );
                                               SetResistance( ResistanceType.Fire, 25 );
                                               SetResistance( ResistanceType.Energy, 25 );
                                               SetResistance( ResistanceType.Poison, 100 );
                                               Fame = 1200;
                                               Karma = 500;
                                               VirtualArmor = 40;
     
                                               
                                               ControlSlots = 3;
                                               MinTameSkill = 90;
                                               PackGold( 1000, 1020 );
                                               

                            }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }

public AToxicMount( Serial serial ) : base( serial )
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
