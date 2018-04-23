//Created by Needles/Marcus Kane
using System;
using Server;
using Server.Items;


namespace Server.Items
{
              public class HydraHelm: DragonHelm
{
              
              [Constructable]
              public HydraHelm()
{

                          Weight = 3;
                          Name = "Helm of the Hydra";
                          Hue = 1673;
              
              Attributes.AttackChance = 15;
              Attributes.BonusDex = 10;
              Attributes.BonusHits = 35;
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
              Attributes.RegenHits = 25;
              Attributes.RegenMana = 25;
              Attributes.RegenStam = 25;
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
              public HydraHelm( Serial serial ) : base( serial )
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
