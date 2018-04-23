

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Garuda" )]
              public class Garuda : MeerCaptain
              {
                                 [Constructable]
                                    public Garuda() : base()
                            {
                                               Name = "Garuda";
					       Title = "The Jewel Thief";
                                               Hue = 1161;
                                               //Body = 9; // Uncomment these lines and input values
                                               //BaseSoundID = 357; // To use your own custom body and sound.
                                               SetStr( 2000, 4000 );
                                               SetDex( 1500, 3000 );
                                               SetInt( 1000, 2000 );
                                               SetHits( 50000, 100000 );
                                               SetDamage( 25, 50 );
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

						SetSkill( SkillName.EvalInt, 320.1, 330.0 );
						SetSkill( SkillName.Magery, 290.1, 300.0 );
						SetSkill( SkillName.Stealing, 300.1, 401.0 );
						SetSkill( SkillName.Snooping, 400.1, 601.0 );
						SetSkill( SkillName.MagicResist, 575.2, 600.0 );
						SetSkill( SkillName.Tactics, 690.1, 800.0 );
						SetSkill( SkillName.Wrestling, 675.1, 800.0 );
						SetSkill( SkillName.Necromancy, 375.1, 400.0 );
						SetSkill( SkillName.Anatomy, 575.1, 600.0 );
						SetSkill( SkillName.Parry, 675.1, 700.0 );
						
                                               Fame = -30000;
                                               Karma = -500000;
                                               VirtualArmor = 70;

     
                                                 switch ( Utility.Random( 50 ))
			         {
				
				case 0: PackItem( new StolenEarrings() ); break;
				case 1: PackItem( new StolenBracelet() ); break;
                        	case 2: PackItem( new StolenRing() ); break;
                        	case 3: PackItem( new StolenNecklace() ); break;
                        	
                       		{	
			
			
		 }}

                            }
                                 public override bool HasBreath{ get{ return true ; } }
				public override int BreathFireDamage{ get{ return 11; } }
				public override int BreathColdDamage{ get{ return 11; } }
//                public override bool IsScaryToPets{ get{ return true; } }
				public override bool AutoDispel{ get{ return true; } }
                public override bool BardImmune{ get{ return true; } }
                public override bool Unprovokable{ get{ return true; } }
                public override Poison HitPoison{ get{ return Poison. Lethal ; } }
                public override bool AlwaysMurderer{ get{ return true; } }
//				public override bool IsScaredOfScaryThings{ get{ return false; }}
		public override bool CanRummageCorpses{ get{ return true; } }

public Garuda( Serial serial ) : base( serial )
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
