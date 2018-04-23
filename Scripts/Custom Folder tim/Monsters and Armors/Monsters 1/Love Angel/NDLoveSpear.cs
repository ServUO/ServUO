
using System;
using Server;

namespace Server.Items

{
              
              public class NDLoveSpear : Spear
              {
              public override int AosMinDamage{ get{ return 20; } }
              public override int AosMaxDamage{ get{ return 25; } }
              public override int DefMaxRange{ get{ return 25; } }

                      [Constructable]
                      public NDLoveSpear() 
                      {
                                        Weight = 5;
                                        Name = "Spear of Love";
                                        Hue = 2092;
              
                                        WeaponAttributes.HitDispel = 75;
                                        WeaponAttributes.HitFireball = 75;
                                        WeaponAttributes.HitHarm = 75;
                                        WeaponAttributes.HitLightning = 75;
                                        WeaponAttributes.HitLowerAttack = 75;
                                        WeaponAttributes.HitLowerDefend = 75;
                                        WeaponAttributes.HitMagicArrow = 75;
                                        WeaponAttributes.SelfRepair = 5;
              
                                        Attributes.AttackChance = 15;
                                        Attributes.DefendChance = 15;
                                       Attributes.ReflectPhysical = 15;
                                        Attributes.WeaponDamage = 75;
                                        Attributes.WeaponSpeed = 75;
              
                                    }
              
                      public NDLoveSpear( Serial serial ) : base( serial )  
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
