

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of a Night Guard" )]
              public class NightGuard : BaseCreature
              {
                                 [Constructable]
                                    public NightGuard() : base( AIType.AI_Melee, FightMode.Aggressor, 14, 1, 0.8, 1.6 ){
                                              
                                               SpeechHue = Utility.RandomDyedHue();
                                               Title = "A Night Guard";
                                     
                                               Name = NameList.RandomName( "male" );
                                               Hue = Utility.RandomSkinHue();
                                               Body = 0x190; 
                                               InitStats( 115, 100, 85 );
                                               SetHits( 100 );
                                               SetDamage( 22 );

                                               Skills[SkillName.Anatomy].Base = 120.0;
                                               Skills[SkillName.Tactics].Base = 120.0;
                                               Skills[SkillName.Fencing].Base = 120.0;
                                               Skills[SkillName.MagicResist].Base = 120.0;
                                               Skills[SkillName.DetectHidden].Base = 100.0;
                                     
                                               StuddedChest sChest = new StuddedChest();
                                               sChest.Hue = 1899;
                                               AddItem(sChest);
                                               StuddedArms sArms = new StuddedArms();
                                               sArms.Hue = 1899;
                                               AddItem(sArms);
                                               StuddedGloves sGloves = new StuddedGloves();
                                               sGloves.Hue = 1899;
                                               AddItem(sGloves);
                                               StuddedGorget sGorget = new StuddedGorget();
                                               sGorget.Hue = 1899;
                                               AddItem(sGorget);
                                               StuddedLegs sLegs = new StuddedLegs();
                                               sLegs.Hue = 1899;
                                               AddItem(sLegs);
                                               AddItem(new Boots(1899));
                                               AddItem(new SkullCap(1899));

                                               Kryss k = new Kryss();
                                               k.Hue = 1899;
                                               AddItem(k);
                                               
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
                                     
                                               Fame = 3500;
                                               Karma = 3500;

                                               VirtualArmor = 40;
                                               
     
                                               PackGold( 700, 900 );
                                                

                            }


                  public NightGuard( Serial serial ) : base( serial )
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
