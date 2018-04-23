//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class HellsAxe : LargeBattleAxe
              {
              public override int ArtifactRarity{ get{ return 666; } } 
              public override int AosMinDamage{ get{ return 25; } }
              public override int AosMaxDamage{ get{ return 45; } }
              
                      [Constructable]
                      public HellsAxe() 
                      {
                                        Weight = 2;
                                        Name = "Hells Axe";
                                        Hue = 1157;
              
                                        WeaponAttributes.DurabilityBonus = 100;
                                        WeaponAttributes.HitHarm = 85;
                                        WeaponAttributes.HitLightning = 60;
                                        WeaponAttributes.HitMagicArrow = 75;
                                        WeaponAttributes.LowerStatReq = 100;
                                        WeaponAttributes.SelfRepair = 10;
                                        WeaponAttributes.UseBestSkill = 1;
              
                                        Attributes.AttackChance = 25;
                                        Attributes.BonusDex = 50;
                                        Attributes.BonusHits = 100;
                                       Attributes.CastRecovery = 1;
                                        Attributes.CastSpeed = 1;
                                        Attributes.DefendChance = 25;
                                        Attributes.LowerManaCost = 10;
                                        Attributes.LowerRegCost = 45;
                                        Attributes.Luck = 1000;
                                        Attributes.SpellChanneling = 1;
                                        Attributes.SpellDamage = 15;
                                        Attributes.WeaponDamage = 25;
                                        Attributes.WeaponSpeed = 25;
              
                                    }
              
                      public HellsAxe( Serial serial ) : base( serial )  
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
