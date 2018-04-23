//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class TuffHammer : HammerPick
              {
              public override int ArtifactRarity{ get{ return 666; } } 
              public override int AosMinDamage{ get{ return 999; } }
              public override int AosMaxDamage{ get{ return 999; } }
              
                      [Constructable]
                      public TuffHammer() 
                      {
                                        Weight = 0;
                                        Name = "TuffHammer";
                                        Hue = 1157;
              
                                        WeaponAttributes.DurabilityBonus = 100;
                                        WeaponAttributes.HitHarm = 100;
                                        WeaponAttributes.HitLeechHits = 85;
                                        WeaponAttributes.HitLightning = 100;
                                        WeaponAttributes.HitMagicArrow = 100;
                                        WeaponAttributes.SelfRepair = 10;
                                        WeaponAttributes.UseBestSkill = 1;
              
                                        Attributes.AttackChance = 50;
                                        Attributes.BonusDex = 10;
                                        Attributes.BonusHits = 100;
                                        Attributes.BonusInt = 10;
                                        Attributes.BonusMana = 100 ;
                                        Attributes.BonusStam = 100;
                                       Attributes.CastRecovery = 1;
                                        Attributes.CastSpeed = 1;
                                        Attributes.DefendChance = 5;
                                        Attributes.LowerManaCost = 10;
                                        Attributes.LowerRegCost = 10;
                                        Attributes.Luck = 100;
                                       Attributes.ReflectPhysical = 25;
                                        Attributes.RegenHits = 5;
                                        Attributes.RegenMana = 5;
                                        Attributes.RegenStam = 5;
                                        Attributes.SpellDamage = 25;
                                        Attributes.WeaponDamage = 75;
                                        Attributes.WeaponSpeed = 25;
              
                                                      Slayer = SlayerName.Silver;
                                                      LootType = LootType.Blessed;
                                    }
              
                      public TuffHammer( Serial serial ) : base( serial )  
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
