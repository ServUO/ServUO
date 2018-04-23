//Created by Script Creator
using System;
using Server;

namespace Server.Items
{
              public class OblivionShield: ChaosShield
{
              
              [Constructable]
              public OblivionShield()
{

                          Name = "Shield of Oblivion";
                          Hue = 1261;
              
              Attributes.AttackChance = 25;
              Attributes.BonusDex = 15;
              Attributes.BonusHits = 30;
              Attributes.BonusInt = 15;
              Attributes.BonusMana = 30;
              Attributes.BonusStam = 30;
              Attributes.CastRecovery = 3;
              Attributes.CastSpeed = 3;
              Attributes.DefendChance = 15;
              Attributes.LowerManaCost = 15;
              Attributes.LowerRegCost = 20;
              Attributes.NightSight = 1;
              Attributes.ReflectPhysical = 15;
              Attributes.RegenHits = 5;
              Attributes.RegenMana = 5;
              Attributes.RegenStam = 5;
              Attributes.SpellChanneling = 1;
              Attributes.SpellDamage = 15;
              Attributes.WeaponDamage = 30;
              ArmorAttributes.SelfRepair = 5;
              ColdBonus = 20;
              EnergyBonus = 20;
              FireBonus = 20;
              PhysicalBonus = 20;
              PoisonBonus = 20;
              StrBonus = 15;
                  }
              public OblivionShield( Serial serial ) : base( serial )
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
