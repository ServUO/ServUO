
using System;
using Server;

namespace Server.Items

{
              
              public class DLoveSpear : Spear
              {
              public override int AosMinDamage{ get{ return 20; } }
              public override int AosMaxDamage{ get{ return 25; } }
              public override int DefMaxRange{ get{ return 3; } }

                      [Constructable]
                      public DLoveSpear() 
                      {
                                        Weight = 5;
                                        Name = "Spear of Love";
                                        Hue = 2092;
              
                                        WeaponAttributes.HitDispel = Utility.Random( 1, 75 );
                                        WeaponAttributes.HitFireball = Utility.Random( 1, 75 );
                                        WeaponAttributes.HitHarm = Utility.Random( 1, 75 );
                                        WeaponAttributes.HitLightning = Utility.Random( 1, 75 );
                                        WeaponAttributes.HitLowerAttack = Utility.Random( 1, 75 );
                                        WeaponAttributes.HitLowerDefend = Utility.Random( 1, 75 );
                                        WeaponAttributes.HitMagicArrow = Utility.Random( 1, 75 );
                                        WeaponAttributes.SelfRepair = 5;
              
                                        Attributes.AttackChance = 15;
                                        Attributes.DefendChance = 15;
                                       Attributes.ReflectPhysical = 15;
                                        Attributes.WeaponDamage = Utility.Random( 1, 75 );
                                        Attributes.WeaponSpeed = Utility.Random( 1, 75 );
              
                                    }
              
                      public DLoveSpear( Serial serial ) : base( serial )  
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
