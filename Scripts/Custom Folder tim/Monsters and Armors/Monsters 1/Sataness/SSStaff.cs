
using System;
using Server;

namespace Server.Items

{
              
              public class SSStaff : DoubleBladedStaff
              {
              public override int AosMinDamage{ get{ return 30; } }
              public override int AosMaxDamage{ get{ return 45; } }
              public override int DefMaxRange{ get{ return 3; } }

                      [Constructable]
                      public SSStaff() 
                      {
                                        Weight = 5;
                                        Name = "Sataness Sin Staff of Temptation";
                                        Hue = 2255;
              
                                        WeaponAttributes.HitDispel = Utility.Random( 1, 50 );
                                        WeaponAttributes.HitFireball = Utility.Random( 1, 50 );
                                        WeaponAttributes.HitHarm = Utility.Random( 1, 50 );
                                        WeaponAttributes.HitLightning = Utility.Random( 1, 50 );
                                        WeaponAttributes.HitLowerAttack = Utility.Random( 1, 50 );
                                        WeaponAttributes.HitLowerDefend = Utility.Random( 1, 50 );
                                        WeaponAttributes.HitMagicArrow = Utility.Random( 1, 50 );
                                        WeaponAttributes.SelfRepair = 5;
              
                                        Attributes.AttackChance = Utility.Random( 1, 25 );
                                        Attributes.DefendChance = Utility.Random( 1, 25 );
                                       Attributes.ReflectPhysical = Utility.Random( 1, 35 );
                                        Attributes.WeaponDamage = Utility.Random( 1, 120 );
                                        Attributes.WeaponSpeed = Utility.Random( 1, 75 );
              
                                    }
              
                      public SSStaff( Serial serial ) : base( serial )  
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
