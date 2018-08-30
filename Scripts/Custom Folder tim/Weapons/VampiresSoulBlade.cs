//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class VampiresSoulBlade : Kryss
              {
              public override int ArtifactRarity{ get{ return 666; } } 
              public override int AosMinDamage{ get{ return 15; } }
              public override int AosMaxDamage{ get{ return 30; } }
              
                      [Constructable]
                      public VampiresSoulBlade() 
                      {
                                        Weight = 0;
                                        Name = "Vampires Soul Blade";
                                        Hue = 1157;
              
                                        WeaponAttributes.DurabilityBonus = 150;
                                        WeaponAttributes.HitHarm = 65;
                                        WeaponAttributes.HitLeechHits = 100;
                                        WeaponAttributes.SelfRepair = 10;
              
                                        Attributes.AttackChance = 50;
                                        Attributes.BonusDex = 25;
                                        Attributes.DefendChance = 50;
                                        Attributes.NightSight = 1;
                                        Attributes.SpellChanneling = 1;
                                        Attributes.WeaponDamage = 25;
                                        Attributes.WeaponSpeed = 25;
              
                                    }
              
                      public VampiresSoulBlade( Serial serial ) : base( serial )  
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
