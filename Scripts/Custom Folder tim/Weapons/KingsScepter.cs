//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class KingsScepter : Scepter
              {
              public override int ArtifactRarity{ get{ return 1010; } } 
              public override int AosMinDamage{ get{ return 20; } }
              public override int AosMaxDamage{ get{ return 25; } }
              
                      [Constructable]
                      public KingsScepter() 
                      {
                                        Weight = 5;
                                        Name = "Kings Scepter";
                                        Hue = 32;
              
                                        WeaponAttributes.DurabilityBonus = 100;
                                        WeaponAttributes.HitEnergyArea = 15;
                                        WeaponAttributes.HitFireArea = 50;
                                        WeaponAttributes.HitHarm = 55;
                                        WeaponAttributes.HitLeechHits = 25;
                                        WeaponAttributes.HitLeechMana = 25;
                                        WeaponAttributes.HitLeechStam = 25;
                                        WeaponAttributes.HitLightning = 65;
                                        WeaponAttributes.HitLowerAttack = 10;
                                        WeaponAttributes.HitMagicArrow = 25;
                                        WeaponAttributes.HitPhysicalArea = 10;
                                        WeaponAttributes.HitPoisonArea = 10;
                                        WeaponAttributes.SelfRepair = 10;
              
                                        Attributes.AttackChance = 10;
                                        Attributes.DefendChance = 10;
                                        Attributes.SpellChanneling = 1;
              
                                    }
              
                      public KingsScepter( Serial serial ) : base( serial )  
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
