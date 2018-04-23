//Created by Sarenbou
using System;
using Server;

namespace Server.Items

{
              
              public class PimpCane : Scepter
              {
              public override int AosMinDamage{ get{ return 25; } }
              public override int AosMaxDamage{ get{ return 35; } }
              
                      [Constructable]
                      public PimpCane() 
                      {
                                        Weight = 1;
                                        Name = "Pimp Cane";
                                        Hue = 1281;
              
                                        WeaponAttributes.DurabilityBonus = 1500;
                                        WeaponAttributes.HitLeechHits = 50;
                                        WeaponAttributes.HitLeechMana = 50;
                                        WeaponAttributes.HitLeechStam = 50;
                                        WeaponAttributes.HitLightning = 75;
                                        WeaponAttributes.SelfRepair = 10;
                                       
                                        Attributes.AttackChance = 15;
                                        Attributes.BonusDex = 20;
                                        
                                        Attributes.BonusStr = 20;
                                        Attributes.DefendChance = 15;
                                        Attributes.RegenHits = 5;
                                        Attributes.RegenMana = 5;
                                        Attributes.RegenStam = 5;
                                        Attributes.SpellChanneling = 1;
                                        Attributes.SpellDamage = 20;
                                        Attributes.WeaponDamage = 20;
                                        Attributes.WeaponSpeed = 50;
              
                                                      LootType = LootType.Regular;
                                    }
              
                      public PimpCane( Serial serial ) : base( serial )  
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
