// Created by Dudus

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of the Skeletal Serpent" )]
              public class SkeletalSerpent : Daemon
              {
                                 [Constructable]
                                    public SkeletalSerpent() : base()
                            {
                                               Name = "Skeletal Serpent";
                                               Hue = 65;
                                               Body = 104; // Uncomment these lines and input values
                                               //BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 1500, 2000 );
                                               SetDex( 1500, 2000 );
                                               SetInt( 1500, 2000 );
                                               SetHits( 30000, 40000 );
                                               SetDamage( 20, 25 );
                                               SetDamageType( ResistanceType.Physical, 100 );
                                               SetDamageType( ResistanceType.Cold, 60 );
                                               SetDamageType( ResistanceType.Fire, 60 );
                                               SetDamageType( ResistanceType.Energy, 60 );
                                               SetDamageType( ResistanceType.Poison, 60 );

                                               SetResistance( ResistanceType.Physical, 70 );
                                               SetResistance( ResistanceType.Cold, 70 );
                                               SetResistance( ResistanceType.Fire, 70 );
                                               SetResistance( ResistanceType.Energy, 70 );
                                               SetResistance( ResistanceType.Poison, 70 );
                                               Fame = 30000;
                                               Karma = - 25000;
                                               VirtualArmor = 70;
     
                                                 switch ( Utility.Random( 50 ))
			         {
				case 0: PackItem( new SkeletalSerpentChest() ); break;
				case 1: PackItem( new SkeletalSerpentArms() ); break;
				case 2: PackItem( new SkeletalSerpentGloves() ); break;
                        	case 3: PackItem( new SkeletalSerpentLegs() ); break;
					{	
			
			
		 }}

                            }
                                 public override bool HasBreath{ get{ return true ; } }
                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }

public SkeletalSerpent( Serial serial ) : base( serial )
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
