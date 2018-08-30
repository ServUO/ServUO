

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a Zeus" )]
              public class RSZeus : EtherealWarrior
              {
                                 [Constructable]
                                    public RSZeus() : base()
                            {
                                               Name = "Zeus";
						Title = "Lord of the Heavens";
                                               Hue = 2248;
                                               //Body = 308; // Uncomment these lines and input values
                                               //BaseSoundID = 0x48D; // To use your own custom body and sound.
                                               SetStr( 666 );
                                               SetDex( 666 );
                                               SetInt( 666 );
                                               SetHits( 66666 );
                                               SetDamage( 666 );
                                               SetDamageType( ResistanceType.Physical, 0 );
                                               SetDamageType( ResistanceType.Cold, 666 );
                                               SetDamageType( ResistanceType.Fire, 666 );
                                               SetDamageType( ResistanceType.Energy, 666 );
                                               SetDamageType( ResistanceType.Poison, 666 );

                                               SetResistance( ResistanceType.Physical, 666 );
                                               SetResistance( ResistanceType.Cold, 666 );
                                               SetResistance( ResistanceType.Fire, 666 );
                                               SetResistance( ResistanceType.Energy, 666 );
                                               SetResistance( ResistanceType.Poison, 666 );
                                               Fame = 0;
                                               Karma = 0;
                                               VirtualArmor = 666;
     
                                               PackGold( 5000, 10000 );
						}
				public override void GenerateLoot()
					{		
			switch ( Utility.Random( 60 ))
			{
				case 0: PackItem( new LegsoftheHeavens() ); break;
				case 1: PackItem( new GlovesoftheHeavens() ); break;
				case 2: PackItem( new ArmsoftheHeavens() ); break;
                        case 3: PackItem( new ChestoftheHeavens() ); break;
			case 4: PackItem( new CrescentBladeoftheHeavens() ); break;
			case 5: PackItem( new HelmoftheHeavens() ); break;
                            {

			
		 }}

                            }
                                
                               
                                 public override bool HasBreath{ get{ return true ; } }
                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                                 public override bool AlwaysMurderer{ get{ return true; } }

public RSZeus( Serial serial ) : base( serial )
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
