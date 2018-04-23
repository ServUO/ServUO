//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class AncientBlade : VikingSword
              {
              public override int ArtifactRarity{ get{ return 155; } } 
              public override int AosMinDamage{ get{ return 26; } }
              public override int AosMaxDamage{ get{ return 42; } }
              
                      [Constructable]
                      public AncientBlade() 
                      {
                                        Weight = 5;
                                        Name = "Ancient Blade";
                                        Hue = 0;
              
                                        WeaponAttributes.DurabilityBonus = 1000;
                                        WeaponAttributes.HitLeechHits = 25;
                                        WeaponAttributes.HitLeechMana = 25;
                                        WeaponAttributes.HitLeechStam = 25;
                                        WeaponAttributes.SelfRepair = 100;
                                        WeaponAttributes.UseBestSkill = 1;
              
                                       Attributes.ReflectPhysical = 55;
                                        Attributes.SpellChanneling = 1;
              
                                    }
              
                      public AncientBlade( Serial serial ) : base( serial )  
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
