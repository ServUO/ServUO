//Created by Script Creator
using System;
using Server;

namespace Server.Items

{
              
              public class FireDemonStaff : BlackStaff
              {
              public override int AosMinDamage{ get{ return 20; } }
              public override int AosMaxDamage{ get{ return 24; } }
              
                      [Constructable]
                      public FireDemonStaff() 
                      {
                                        Weight = 5;
                                        Name = "Staff of the Fire Demon";
                                        Hue = 2666;
              
                                        WeaponAttributes.HitDispel = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitFireball = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitHarm = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitLeechHits = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitLeechMana = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitLeechStam = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitLightning = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitLowerAttack = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitLowerDefend = Utility.Random( 1, 10 );
                                        WeaponAttributes.HitMagicArrow = Utility.Random( 1, 10 );
                                        WeaponAttributes.SelfRepair = 5;
              
                                        Attributes.AttackChance = Utility.Random( 1, 20 );
                                        Attributes.DefendChance = Utility.Random( 1, 20 );
                                       Attributes.ReflectPhysical = Utility.Random( 1, 20 );
              
                                    }
              
                      public FireDemonStaff( Serial serial ) : base( serial )  
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
