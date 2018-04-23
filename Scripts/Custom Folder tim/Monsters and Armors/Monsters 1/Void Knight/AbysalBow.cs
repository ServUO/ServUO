//Created by Atown
using System;
using Server;

namespace Server.Items

{
              
              public class AbysalBow : Bow
              {
	      public override int DefMaxRange{ get{ return 10; } }
              public override int ArtifactRarity{ get{ return 1994; } } 
              public override int AosMinDamage{ get{ return 25; } }
              public override int AosMaxDamage{ get{ return 35; } }
              
                      [Constructable]
                      public AbysalBow() 
                      {
                                        Weight = 0;
                                        Name = "Abysal Bow";
                                        Hue = 1931;
              
                                        WeaponAttributes.HitEnergyArea = 50;
                                        WeaponAttributes.HitHarm = 15;
                                        WeaponAttributes.HitLeechHits = 25;
                                        WeaponAttributes.HitLeechMana = 15;
                                        WeaponAttributes.HitLeechStam = 35;
                                        WeaponAttributes.HitLightning = 35;
                                        WeaponAttributes.SelfRepair = 15;
              
                                        Attributes.AttackChance = 15;
                                        Attributes.BonusDex = 15;
                                        Attributes.BonusInt = 10;
                                       Attributes.CastRecovery = 5;
                                        Attributes.CastSpeed = 5;
                                        Attributes.DefendChance = 15;
                                        Attributes.LowerManaCost = 15;
                                        Attributes.LowerRegCost = 15;
                                        Attributes.Luck = 500;
                                        Attributes.WeaponDamage = 25;
                                        Attributes.WeaponSpeed = 45;
              
                                    }
              
                      public AbysalBow( Serial serial ) : base( serial )  
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
