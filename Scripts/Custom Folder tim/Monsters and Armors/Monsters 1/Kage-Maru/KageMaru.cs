// Created by Dudus

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Kage-Maru" )]
              public class KageMaru : MeerCaptain
              {
                                 [Constructable]
                                    public KageMaru() : base()
                            {
                                               Name = "Kage-Maru";
					       Title = "The Tenth Generation";
                                               Hue = 1107;
                                               //Body = 9; // Uncomment these lines and input values
                                               //BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 5000 );
                                               SetDex( 2000 );
                                               SetInt( 250 );
                                               SetHits( 66600 );
                                               SetDamage( 25, 30 );
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
     
                                                 switch ( Utility.Random( 35 ))
			         {
				
				case 0: PackItem( new KageMaruChest() ); break;
				case 1: PackItem( new KageMaruGloves() ); break;
                        	case 2: PackItem( new KageMaruHood() ); break;
                        	case 3: PackItem( new KageMaruMask() ); break;
                        	case 4: PackItem( new KageMaruPants() ); break;
                        	case 5: PackItem( new KageMaruShoes() ); break;
                       		{	
			
			
		 }}

                            }
                                 public override bool HasBreath{ get{ return true ; } }
                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }

public KageMaru( Serial serial ) : base( serial )
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
