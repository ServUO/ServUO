//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class StaffOfTheGods : BlackStaff
              {
              public override int ArtifactRarity{ get{ return 999; } } 
              public override int AosMinDamage{ get{ return 25; } }
              public override int AosMaxDamage{ get{ return 35; } }
              
                      [Constructable]
                      public StaffOfTheGods() 
                      {
                                        Weight = 5;
                                        Name = "Staff Of The Gods";
                                        Hue = 1153;
              
                                        WeaponAttributes.DurabilityBonus = 1000;
                                        WeaponAttributes.HitEnergyArea = 100;
                                        WeaponAttributes.HitHarm = 100;
                                        WeaponAttributes.HitLightning = 100;
                                        WeaponAttributes.SelfRepair = 10;
              
                                        Attributes.BonusDex = 10;
                                        Attributes.BonusInt = 10;
                                       Attributes.CastRecovery = 5;
                                        Attributes.CastSpeed = 5;
                                        Attributes.DefendChance = 25;
                                        Attributes.LowerManaCost = 50;
                                        Attributes.LowerRegCost = 50;
                                        Attributes.Luck = 1000;
                                       Attributes.ReflectPhysical = 25;
                                        Attributes.RegenHits = 10;
                                        Attributes.RegenMana = 10;
                                        Attributes.RegenStam = 10;
                                        Attributes.SpellChanneling = 1;
                                        Attributes.SpellDamage = 50;
                                        Attributes.WeaponDamage = 75;
                                        Attributes.WeaponSpeed = 75;
              
                                                      LootType = LootType.Blessed;
                                    }
              
                      public StaffOfTheGods( Serial serial ) : base( serial )  
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
