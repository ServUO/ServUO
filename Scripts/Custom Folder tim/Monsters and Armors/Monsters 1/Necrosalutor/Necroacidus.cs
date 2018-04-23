//Created by Neptune
using System;
using Server;

namespace Server.Items

{
              
              public class Necroacidus : Hatchet
              { 
              public override int AosMinDamage{ get{ return 19; } }
              public override int AosMaxDamage{ get{ return 20; } }
              
                      [Constructable]
                      public Necroacidus() 
                      {
                                        Weight = 1;
                                       
                                        Name = "Necroacidus";
                                       
              
                                        WeaponAttributes.HitLeechHits = 25;
                                        WeaponAttributes.HitLeechMana = 25;
                                        WeaponAttributes.HitLowerAttack = 50;
                                        WeaponAttributes.HitLeechStam = 25;
                                        WeaponAttributes.HitLowerDefend = 50;
                                        WeaponAttributes.HitHarm = 50;
                                        WeaponAttributes.SelfRepair = 10;
                                         Attributes.WeaponDamage = 200;
                                        Attributes.WeaponSpeed = 100;
              
                                    }
              
                      public Necroacidus( Serial serial ) : base( serial )  
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
