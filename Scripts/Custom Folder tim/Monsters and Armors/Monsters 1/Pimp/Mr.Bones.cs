// Created by Eversoris

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of the Pimp" )]
              public class MrBones : BoneDemon
              {
                                 [Constructable]
                                    public MrBones() : base()
                            {
                                               Name = "Mr. Bones";
					       Title = "The Pimp";
                                               Hue = 1150;
                                               //Body = 9; // Uncomment these lines and input values
                                               //BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 3000 );
                                               SetDex( 5000 );
                                               SetInt( 4000 );
                                               SetHits( 100000 );
                                               SetDamage( 25, 35 );
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
     
                                                 switch ( Utility.Random( 30 ))
			         {
				
				case 0: PackItem( new PimpsFace() ); break;
				case 1: PackItem( new PimpCane() ); break;
                        	case 2: PackItem( new PimpStick() ); break;
                        	case 3: PackItem( new PimpsHands() ); break;
                        	case 4: PackItem( new PimpRobe() ); break;
                        	case 5: PackItem( new ParachutePants() ); break;
                        	case 6: PackItem( new PimpHat() ); break;
                       		{	
			
			
		 }}

                            }
                                 public override bool AutoDispel{ get{ return true; } }
                                 public override bool BardImmune{ get{ return true; } }
                                 public override bool Unprovokable{ get{ return true; } }
                                 public override Poison HitPoison{ get{ return Poison. Lethal ; } }

public MrBones( Serial serial ) : base( serial )
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
