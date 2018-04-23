//Created by Sarenbou
using System;
using Server;

namespace Server.Items

{
              
              public class PimpStick : BlackStaff
              {
              public override int AosMinDamage{ get{ return 25; } }
              public override int AosMaxDamage{ get{ return 35; } }
              
                      [Constructable]
                      public PimpStick() 
                      {
                                        Weight = 0;
                                        Name = "Pimp Stick";
                                        Hue = 1289;
              
                                        WeaponAttributes.DurabilityBonus = 1500;
                                        WeaponAttributes.HitLeechHits = 100;
                                        WeaponAttributes.HitLeechMana = 100;
                                        WeaponAttributes.HitLeechStam = 100;
                                        WeaponAttributes.HitLightning = 125;
                                        WeaponAttributes.SelfRepair = 10;
                                       
                                        Attributes.AttackChance = 25;
                                        Attributes.BonusDex = 15;
                                        Attributes.BonusStr = 15;
                                        Attributes.DefendChance = 25;
                                        Attributes.RegenHits = 5;
                                        Attributes.RegenMana = 5;
                                        Attributes.RegenStam = 5;
                                        Attributes.SpellChanneling = 1;
                                        Attributes.SpellDamage = 15;
                                        Attributes.WeaponDamage = 25;
                                        Attributes.WeaponSpeed = 25;
              
                                                      LootType = LootType.Regular;
                                    }
              
                      public PimpStick( Serial serial ) : base( serial )  
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
