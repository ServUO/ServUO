//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class GoldenSlayer : Halberd
              {
              public override int ArtifactRarity{ get{ return 555; } } 
              public override int AosMinDamage{ get{ return 20; } }
              public override int AosMaxDamage{ get{ return 20; } }
              
                      [Constructable]
                      public GoldenSlayer() 
                      {
                                        Weight = 2;
                                        Name = "Golden Slayer";
                                        Hue = 1174;
              
                                        WeaponAttributes.DurabilityBonus = 550;
                                        WeaponAttributes.HitHarm = 80;
                                        WeaponAttributes.HitLightning = 95;
                                        WeaponAttributes.HitMagicArrow = 50;
                                        WeaponAttributes.SelfRepair = 10;
                                        WeaponAttributes.UseBestSkill = 1;
              
                                        Attributes.AttackChance = 50;
                                        Attributes.BonusDex = 25;
                                        Attributes.SpellChanneling = 1;
              
                                    }
              
                      public GoldenSlayer( Serial serial ) : base( serial )  
                                    {
                                    }
              
                      public override void Serialize( GenericWriter writer )
                                    {
                                                      base.Serialize( writer );
              
                                                      writer.Write( (int) 0 );
                                    }
              
                      public override void Deserialize(GenericReader reader)
                                    {
                                                      base.Deserialize( reader );
                            
                                                      int version = reader.ReadInt();
                                    }
                  }
}
