//Created by Script Creator
using System;
using Server;

namespace Server.Items
{
              public class SuicideArms: LeatherArms
{
              
              [Constructable]
              public SuicideArms()
{

                          Weight = 5;
                          Name = "Suicide Arms";
                          Hue = 1995;
              
              Attributes.AttackChance = 25;
              Attributes.DefendChance = 25;
              Attributes.LowerManaCost = 25;
              Attributes.LowerRegCost = 25;
              Attributes.ReflectPhysical = 25;
              Attributes.RegenHits = 5;
              Attributes.RegenMana = 5;
              Attributes.RegenStam = 5;
              Attributes.SpellDamage = 25;
              ArmorAttributes.LowerStatReq = 100;
              ArmorAttributes.SelfRepair = 5;
              ColdBonus = 25;
              EnergyBonus = 25;
              FireBonus = 25;
              PhysicalBonus = 25;
              PoisonBonus = 25;
                  }
              public SuicideArms( Serial serial ) : base( serial )
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
