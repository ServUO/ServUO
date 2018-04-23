//Created by Needles/Marcus Kane
using System;
using Server;
using Server.Items;


namespace Server.Items
{
              public class HydraShield: OrderShield
{
              
              [Constructable]
              public HydraShield()
{

                          Weight = 3;
                          Name = "Shield of the Hydra";
                          Hue = 1673;
              
              Attributes.AttackChance = 15;
              Attributes.BonusDex = 10;
              Attributes.BonusHits = 35;
	      Attributes.BonusStr = 20;
              Attributes.BonusInt = 15;
              Attributes.BonusMana = 10;
              Attributes.BonusStam = 75;
              Attributes.CastRecovery = 4;
              Attributes.CastSpeed = 2;
              Attributes.DefendChance = 15;
              Attributes.LowerManaCost = 10;
              Attributes.LowerRegCost = 30;
              Attributes.NightSight = 1;
              Attributes.ReflectPhysical = 10;
              Attributes.RegenHits = 10;
              Attributes.RegenMana = 5;
              Attributes.RegenStam = 1;
              Attributes.SpellChanneling = 1;
              Attributes.SpellDamage = 20;
              Attributes.WeaponDamage = 20;
	      Attributes.WeaponSpeed = 20;
              ArmorAttributes.DurabilityBonus = 255;
              ArmorAttributes.SelfRepair = 5;
              ColdBonus = 5;
              EnergyBonus = 5;
              FireBonus = 5;
              PhysicalBonus = 5;
              PoisonBonus = 5;
                  }
              public HydraShield( Serial serial ) : base( serial )
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
