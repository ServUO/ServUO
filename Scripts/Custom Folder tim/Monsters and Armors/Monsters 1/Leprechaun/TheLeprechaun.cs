//Created by Cherokee/Mule II aka Hotshot

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a Leprechaun" )]
              public class LeprechaunOrc : OrcishMage
              {
                                 [Constructable]
                                    public LeprechaunOrc() : base()
                            {
                                               Name = "Leprechaun";
                                               Title = "Protector Of Luck";
                                               Hue = 69;
                                               //Body = 140; // Uncomment these lines and input values
                                               //BaseSoundID = 0x45A; // To use your own custom body and sound.
                                               SetStr( 2000 );
                                               SetDex( 1000);
                                               SetInt( 500 );
                                               SetHits( 50000);
                                               SetDamage( 15, 20 );
                                               SetDamageType( ResistanceType.Physical, 100 );
                                               SetDamageType( ResistanceType.Cold, 70 );
                                               SetDamageType( ResistanceType.Fire, 70 );
                                               SetDamageType( ResistanceType.Energy, 70 );
                                               SetDamageType( ResistanceType.Poison, 100 );

                                               SetResistance( ResistanceType.Physical, 100 );
                                               SetResistance( ResistanceType.Cold, 70 );
                                               SetResistance( ResistanceType.Fire, 70 );
                                               SetResistance( ResistanceType.Energy, 70 );
                                               SetResistance( ResistanceType.Poison, 100 );
                                               Fame = 5000;
                                               Karma = -5000;
                                               VirtualArmor = 70;
     
                                               PackGold( 2000, 3000 );
 				
		 switch ( Utility.Random( 25 ))
            {                                   
            	case 0: PackItem( new GorgetOfTheLeprechaun() ); break;
            	case 1: PackItem( new HelmOfTheLeprechaun() ); break;
		case 2: PackItem( new ArmsOfTheLeprechaun() ); break;
		case 3: PackItem( new ChestOfTheLeprechaun() ); break;
		case 4: PackItem( new GlovesOfTheLeprechaun() ); break;
		case 5: PackItem( new LegsOfTheLeprechaun() ); break;
		case 6: PackItem( new LeprechaunProtection() ); break;
		case 7: PackItem( new ShillelaghOfTheLeprechaun() ); break;
		case 8: PackItem( new HoodedRobeOfTheLeprechaun() ); break;
		}

                            }
                                 public override bool HasBreath{ get{ return true ; } }
                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }

public LeprechaunOrc( Serial serial ) : base( serial )
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
