//Created by Neptune
using System;
using Server;

namespace Server.Items

{
              
              public class KronikAxe : Hatchet
              { 
              public override int AosMinDamage{ get{ return 39; } }
              public override int AosMaxDamage{ get{ return 45; } }
              
                      [Constructable]
                      public KronikAxe() 
                      {
                                        Weight = 1;
                                       
                                        Name = "Axe of Kronik";
                                       
              
                                       WeaponAttributes.HitColdArea = 100;
                                        WeaponAttributes.HitLeechHits = 25;
                                        WeaponAttributes.HitLeechMana = 25;
                                        WeaponAttributes.HitFireArea = 100;
                                        WeaponAttributes.HitLeechStam = 25;
                                        WeaponAttributes.HitFireball = 50;
                                        WeaponAttributes.HitHarm = 50;
                                        WeaponAttributes.SelfRepair = 10;
                                        WeaponAttributes.UseBestSkill = 1;
              
                                        Attributes.AttackChance = 20;
                                        Attributes.BonusDex = 30;
                                       Attributes.CastRecovery = 4;
                                        Attributes.CastSpeed = 3;
                                        Attributes.DefendChance = 20;
                                        Attributes.Luck = 100;
                                        Attributes.RegenHits = 3;
                                        Attributes.SpellChanneling = 1;
                                        Attributes.SpellDamage = 20;
                                         Attributes.WeaponDamage = 50;
                                        Attributes.WeaponSpeed = 20;
              
                                    }
              
                      public KronikAxe( Serial serial ) : base( serial )  
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
