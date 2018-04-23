using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " Corpse of Ser Rodrick" )]
              public class SerRodrick : BaseCreature
              {
                                 [Constructable]
                  public SerRodrick()
                      : base(AIType.AI_Melee, FightMode.Aggressor, 14, 1, 0.8, 1.6)
                            {
                                        SpeechHue = Utility.RandomDyedHue();
                                        Title = "Captain Of The Night Guard";
                                        
                                              
                                               Name = "Rodrick";
                                               Hue = 1010;
                                               Body = 0x190;

                                               PlateChest pChest = new PlateChest();
                                               pChest.Hue = 1899; 
                                               AddItem(pChest);
                                               PlateArms pArms = new PlateArms();
                                               pArms.Hue = 1899; 
                                               AddItem(pArms);
                                               PlateGloves pGloves = new PlateGloves();
                                               pGloves.Hue = 1899; 
                                               AddItem(pGloves);
                                               PlateGorget pGorget = new PlateGorget();
                                               pGorget.Hue = 1899;
                                               AddItem(pGorget);
                                               PlateLegs pLegs = new PlateLegs();
                                               pLegs.Hue = 1899;
                                               AddItem(pLegs);
                                               PlateHelm pHelm = new PlateHelm();
                                               pHelm.Hue = 1899; 
                                               AddItem(pHelm);
                                               AddItem(new Boots(1899));
                                               
                                               SetStr( 340 );
                                               SetDex( 500 );
                                               SetInt( 80 );
                                               SetHits( 400 );
                                               SetDamage( 40 );

                                               Skills[SkillName.Anatomy].Base = 120.0;
                                               Skills[SkillName.Tactics].Base = 120.0;
                                               Skills[SkillName.Swords].Base = 120.0;
                                               Skills[SkillName.MagicResist].Base = 120.0;
                                               Skills[SkillName.DetectHidden].Base = 100.0;
                                               
                                               Fame = 7000;
                                               Karma = 7000;

                                               Utility.AssignRandomHair(this);
                                               if (Utility.RandomBool())
                                                   Utility.AssignRandomFacialHair(this, HairHue);


                                               Cutlass c = new Cutlass();
                                               c.Hue = 1899;
                                               AddItem(c);
                                               
                                               

                                               HeaterShield h = new HeaterShield();
                                               h.Hue = 1899;
                                               AddItem(h);
                                               
                                                
                                     
                                                PackGold( 500, 800 );
                                                

                            }


public SerRodrick( Serial serial ) : base( serial )
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
