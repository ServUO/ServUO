//Created by Cherokee/Mule II aka Hotshot
using System;
using Server;

namespace Server.Items

{
              
              public class TwistedScythe : Scythe
              {
              public override int ArtifactRarity{ get{ return 1408; } } 
              public override int AosMinDamage{ get{ return 40; } }
              public override int AosMaxDamage{ get{ return 50; } }
              
                      [Constructable]
                      public TwistedScythe() 
                      {
                                        Weight = 1;
                                        Layer = Layer.OneHanded;
                                        Name = "Scythe of Twisted Metal";
              
                                        WeaponAttributes.HitFireball = 80;
					WeaponAttributes.HitFireArea = 80;
                                        WeaponAttributes.HitLeechHits = 80;
                                        WeaponAttributes.HitLeechMana = 80;
                                        WeaponAttributes.HitHarm = 80;
                                        WeaponAttributes.HitLeechStam = 80;
                                        WeaponAttributes.HitPoisonArea = 100;
                                        WeaponAttributes.SelfRepair = 10;
              
                                        Attributes.AttackChance = 20;
                                        Attributes.BonusStr = 25;
                                        Attributes.DefendChance = 20;
                                        Attributes.RegenHits = 5;
					Attributes.RegenMana = 5;
					Attributes.RegenStam = 5;
                                        Attributes.SpellChanneling = 1;
                                        Attributes.SpellDamage = 25;
                                         Attributes.WeaponDamage = 50;
                                        Attributes.WeaponSpeed = 25;
              
                                      Slayer = SlayerName.ElementalBan;
                                    }
              
                      public TwistedScythe( Serial serial ) : base( serial )  
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
