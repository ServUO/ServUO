//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class Champion : LargeBattleAxe
              {
              public override int ArtifactRarity{ get{ return 999; } } 
              public override int AosMinDamage{ get{ return 10; } }
              public override int AosMaxDamage{ get{ return 25; } }
              
                      [Constructable]
                      public Champion() 
                      {
                                        Weight = 1;
                                        Name = "Champion";
                                        Hue = 1153;
              
                                        WeaponAttributes.DurabilityBonus = 100;
                                       WeaponAttributes.HitColdArea = 50;
                                        WeaponAttributes.HitEnergyArea = 50;
                                        WeaponAttributes.HitFireArea = 50;
                                        WeaponAttributes.HitPhysicalArea = 50;
                                        WeaponAttributes.HitPoisonArea = 50;
                                        WeaponAttributes.SelfRepair = 10;
              
                                        Attributes.AttackChance = 25;
                                        Attributes.BonusDex = 5;
                                        Attributes.BonusInt = 5;
                                       Attributes.CastRecovery = 1;
                                        Attributes.CastSpeed = 1;
                                        Attributes.Luck = 500;
                                       Attributes.ReflectPhysical = 25;
                                        Attributes.SpellChanneling = 1;
              
                                                      LootType = LootType.Blessed;
                                    }
              
                      public Champion( Serial serial ) : base( serial )  
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
