//Created by Cherokee/Mule II aka Hotshot
using System;
using Server;

namespace Server.Items

{
              
              public class ShillelaghOfTheLeprechaun : Club
              {
              public override int ArtifactRarity{ get{ return 777; } } 
              public override int AosMinDamage{ get{ return 15; } }
              public override int AosMaxDamage{ get{ return 20; } }
              
                      [Constructable]
                      public ShillelaghOfTheLeprechaun() 
                      {
                                        Weight = 1;
                                       
                                        Name = "Shillelagh Of The Leprechaun";
                                       
              
                                       WeaponAttributes.HitColdArea = 50;
                                        WeaponAttributes.HitLeechHits = 50;
                                        WeaponAttributes.HitLeechMana = 50;
                                        WeaponAttributes.HitHarm = 50;
                                        WeaponAttributes.HitLeechStam = 50;
                                        WeaponAttributes.HitPoisonArea = 100;
                                        WeaponAttributes.ResistPoisonBonus = 100;
                                        WeaponAttributes.SelfRepair = 20;
                                        WeaponAttributes.UseBestSkill = 1;
              
                                        Attributes.AttackChance = 20;
                                        Attributes.BonusDex = 30;
                                       Attributes.CastRecovery = 4;
                                        Attributes.CastSpeed = 3;
                                        Attributes.DefendChance = 20;
                                        Attributes.Luck = 1000;
                                        Attributes.RegenHits = 3;
                                        Attributes.SpellChanneling = 1;
                                        Attributes.SpellDamage = 20;
                                         Attributes.WeaponDamage = 70;
                                        Attributes.WeaponSpeed = 70;
              
                                                      Slayer = SlayerName.DaemonDismissal;
                                    }
              
                      public ShillelaghOfTheLeprechaun( Serial serial ) : base( serial )  
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
