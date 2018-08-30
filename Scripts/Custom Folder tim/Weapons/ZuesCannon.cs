//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class ZuesCannon : CompositeBow
              {
              public override int ArtifactRarity{ get{ return 5050; } } 
              public override int AosMinDamage{ get{ return 25; } }
              public override int AosMaxDamage{ get{ return 50; } }
              
                      [Constructable]
                      public ZuesCannon() 
                      {
                                        Weight = 1;
                                        Name = "Zues Cannon";
                                        Hue = 1174;
              
                                        WeaponAttributes.DurabilityBonus = 1000;
                                        WeaponAttributes.HitEnergyArea = 100;
                                        WeaponAttributes.HitLightning = 100;
                                        WeaponAttributes.SelfRepair = 10;
              
                                        Attributes.AttackChance = 50;
                                        Attributes.BonusDex = 25;
                                        Attributes.BonusHits = 25;
                                        Attributes.DefendChance = 55;
                                        Attributes.SpellChanneling = 1;
              
                                    }
              
                      public ZuesCannon( Serial serial ) : base( serial )  
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
