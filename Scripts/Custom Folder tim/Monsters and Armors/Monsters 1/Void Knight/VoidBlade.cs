//Created by Atown
using System;
using Server;

namespace Server.Items

{
              
              public class VoidBlade : Katana
              {
	      public override int DefMaxRange{ get{ return 3; } }
              public override int ArtifactRarity{ get{ return 1994; } } 
              public override int AosMinDamage{ get{ return 15; } }
              public override int AosMaxDamage{ get{ return 25; } }
              
                      [Constructable]
                      public VoidBlade() 
                      {
                                        Weight = 0;
                                        Name = "Void Blade";
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
                                        Attributes.WeaponDamage = 90;
                                        Attributes.WeaponSpeed = 45;
              
                                    }
              
                      public VoidBlade( Serial serial ) : base( serial )  
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
