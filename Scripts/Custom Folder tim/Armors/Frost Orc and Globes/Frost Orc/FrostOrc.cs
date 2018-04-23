// Created by Script Creator

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a Frost Orc" )]
              public class FrostOrc : OrcishMage
              {
                                 [Constructable]
                                    public FrostOrc() : base()
                            {
                                               Name = "Frost Orc";
                                               Hue = 1153;
                                               //Body = 140; // Uncomment these lines and input values
                                               //BaseSoundID = 0x45A; // To use your own custom body and sound.
                                               SetStr( 100 );
                                               SetDex( 200 );
                                               SetInt( 500 );
                                               SetHits( 400 );
                                               SetDamage( 50 );
                                               SetDamageType( ResistanceType.Physical, 100 );
                                               SetDamageType( ResistanceType.Cold, 100 );
                                               SetDamageType( ResistanceType.Fire, 0 );
                                               SetDamageType( ResistanceType.Energy, 0 );
                                               SetDamageType( ResistanceType.Poison, 100 );

                                               SetResistance( ResistanceType.Physical, 100 );
                                               SetResistance( ResistanceType.Cold, 100 );
                                               SetResistance( ResistanceType.Fire, 0 );
                                               SetResistance( ResistanceType.Energy, 0 );
                                               SetResistance( ResistanceType.Poison, 100 );
                                               Fame = 500;
                                               Karma = -5000;
                                               VirtualArmor = 70;
                                               
                                                switch ( Utility.Random( 10 ))
                                               {                                   
            	                                   case 0: AddItem( new RandomSnowGlobeDeed() ); break;
			                         
                                               }
                                            }
                                               public override void GenerateLoot()
                                            {
                                              
                                               PackGold( 250, 500 );
                                               PackItem( new Longsword() );
                                               

                            }
                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Deadly ; } }

public FrostOrc( Serial serial ) : base( serial )
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
